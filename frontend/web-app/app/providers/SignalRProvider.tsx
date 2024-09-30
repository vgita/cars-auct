'use client';

import { useAuctionStore } from '@/hooks/useAuctionStore';
import { useBidsStore } from '@/hooks/useBidsStore';
import { Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { useParams } from 'next/navigation';
import { ReactNode, useCallback, useEffect, useRef } from 'react';

type Props = {
	children: ReactNode;
};

export default function SignalRProvider({ children }: Props) {
	const connection = useRef<HubConnection | null>(null);
	const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
	const addBid = useBidsStore((state) => state.addBid);
	const params = useParams<{ id: string }>();

	const handleBidPlaced = useCallback(
		(bid: Bid) => {
			if (bid.bidStatus.includes('Accepted')) {
				setCurrentPrice(bid.auctionId, bid.amount);
			}

			if (params.id === bid.auctionId) {
				addBid(bid);
			}
		},
		[setCurrentPrice, addBid, params.id]
	);

	useEffect(() => {
		if (!connection.current) {
			connection.current = new HubConnectionBuilder()
				.withUrl(`http://localhost:6001/notifications`)
				.withAutomaticReconnect()
				.build();

			connection.current
				.start()
				.then(() => 'Connected to notifications hub')
				.catch((error) => console.error(error));
		}

		connection.current.on('BidPlaced', handleBidPlaced);

		return () => {
			if (connection.current) {
				connection.current.off('BidPlaced', handleBidPlaced);
			}
		};
	}, [handleBidPlaced]);

	return children;
}
