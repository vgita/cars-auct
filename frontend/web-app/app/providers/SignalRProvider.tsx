'use client';

import { useAuctionStore } from '@/hooks/useAuctionStore';
import { useBidsStore } from '@/hooks/useBidsStore';
import { Auction, AuctionFinished, Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from 'next-auth';
import { useParams } from 'next/navigation';
import { ReactNode, useCallback, useEffect, useRef } from 'react';
import toast from 'react-hot-toast';
import AuctionCreatedToast from '../components/AuctionCreatedToast';
import { getDetails } from '../actions/auctionActions';
import AuctionFinishedToast from '../components/AuctionFinishedToast';

type Props = {
	children: ReactNode;
	user: User | null;
	notifyUrl: string;
};

export default function SignalRProvider({ children, user, notifyUrl }: Props) {
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

	const handleAuctionCreated = useCallback(
		(auction: Auction) => {
			if (user?.username !== auction.seller) {
				return toast(<AuctionCreatedToast auction={auction} />, {
					duration: 10000
				});
			}
		},
		[user?.username]
	);

	const handleAuctionFinished = useCallback(
		(finishedAuction: AuctionFinished) => {
			const auction = getDetails(finishedAuction.auctionId);
			return toast.promise(
				auction,
				{
					loading: 'Loading...',
					success: (auction) => (
						<AuctionFinishedToast
							auction={auction}
							finishedAuction={finishedAuction}
						/>
					),
					error: () => 'Auction finished'
				},
				{ success: { duration: 10000, icon: null } }
			);
		},
		[]
	);

	useEffect(() => {
		if (!connection.current) {
			connection.current = new HubConnectionBuilder()
				.withUrl(notifyUrl)
				.withAutomaticReconnect()
				.build();

			connection.current
				.start()
				.then(() => 'Connected to notifications hub')
				.catch((error) => console.error(error));
		}

		connection.current.on('BidPlaced', handleBidPlaced);
		connection.current.on('AuctionCreated', handleAuctionCreated);
		connection.current.on('AuctionFinished', handleAuctionFinished);

		return () => {
			if (connection.current) {
				connection.current.off('BidPlaced', handleBidPlaced);
				connection.current.off('AuctionCreated', handleAuctionCreated);
				connection.current.off('AuctionFinished', handleAuctionFinished);
			}
		};
	}, [handleBidPlaced, handleAuctionCreated, handleAuctionFinished, notifyUrl]);

	return children;
}
