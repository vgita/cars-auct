'use client';

import React, { useEffect, useState } from 'react';
import { useShallow } from 'zustand/react/shallow';
import qs from 'query-string';

import AuctionCard from './AuctionCard';
import AppPagination from '../components/AppPagination';
import { getData } from '../actions/auctionActions';
import { Auction, PagedResult } from '@/types';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/useParamsStore';

export default function Listings() {
	const [data, setData] = useState<PagedResult<Auction>>();
	const params = useParamsStore(
		useShallow((state) => ({
			pageNumber: state.pageNumber,
			pageSize: state.pageSize,
			searchTem: state.searchTerm
		}))
	);
	const setParams = useParamsStore((state) => state.setParams);
	const url = qs.stringifyUrl({ url: '', query: params });

	function setPageNumber(pageNumber: number) {
		setParams({ pageNumber });
	}

	useEffect(() => {
		getData(url).then((data) => {
			setData(data);
		});
	}, [url]);

	if (!data) return <h3>Loading...</h3>;

	return (
		<>
			<Filters />
			<div className="grid grid-cols-4 gap-6">
				{data.results.map((auction) => (
					<AuctionCard auction={auction} key={auction.id} />
				))}
			</div>
			<div className="flex justify-center mt-4">
				<AppPagination
					currentPage={params.pageNumber}
					pageCount={data.pageCount}
					pageChange={setPageNumber}
				/>
			</div>
		</>
	);
}