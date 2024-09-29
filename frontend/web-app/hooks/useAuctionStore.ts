import { Auction, PagedResult } from '@/types';
import { create } from 'zustand';

type State = {
	auctions: Auction[];
	totalCount: number;
	pageCount: number;
};

type Auctions = {
	setData: (data: PagedResult<Auction>) => void;
	setCurrentPrice: (auctionId: string, price: number) => void;
};

const initialState: State = {
	auctions: [],
	totalCount: 0,
	pageCount: 0
};

export const useAuctionStore = create<State & Auctions>((set) => ({
	...initialState,
	setData: (data: PagedResult<Auction>) => {
		set(() => ({
			auctions: data.results,
			totalCount: data.totalCount,
			pageCount: data.pageCount
		}));
	},

	setCurrentPrice: (auctionId: string, amount: number) => {
		set((state) => ({
			auctions: state.auctions.map((auction) =>
				auction.id === auctionId
					? { ...auction, currentHighBid: amount }
					: auction
			)
		}));
	}
}));
