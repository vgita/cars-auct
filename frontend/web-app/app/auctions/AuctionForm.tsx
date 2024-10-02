/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { Button } from 'flowbite-react';
import { FieldValues, useForm } from 'react-hook-form';
import { useEffect } from 'react';
import Input from '../components/Input';
import DateInput from '../components/DateInput';
import { createAuction, updateAuction } from '../actions/auctionActions';
import { usePathname, useRouter } from 'next/navigation';
import toast from 'react-hot-toast';
import { Auction } from '@/types';

type Props = {
	auction?: Auction;
};

export default function AuctionForm({ auction }: Props) {
	const router = useRouter();
	const pathname = usePathname();
	const {
		control,
		handleSubmit,
		setFocus,
		reset,
		formState: { isSubmitting, isValid }
	} = useForm({
		mode: 'onTouched'
	});
	const isCreate = pathname === '/auctions/create';

	useEffect(() => {
		if (auction) {
			const { make, model, color, year, mileage } = auction;
			reset({
				make,
				model,
				color,
				year,
				mileage
			});
		}
		setFocus('make');
	}, [setFocus, auction, reset]);

	async function onSubmit(data: FieldValues) {
		try {
			let id = '';
			let res: any;

			if (isCreate) {
				res = await createAuction(data);
				id = res.id;
			} else if (auction) {
				res = await updateAuction(auction.id, data);
				id = auction.id;
			}

			if (res.error) {
				throw res.error;
			}

			router.push(`/auctions/details/${id}`);
		} catch (error: any) {
			toast.error(error.status + ' ' + error.message);
		}
	}

	return (
		<form className="flex flex-col mt-3" onSubmit={handleSubmit(onSubmit)}>
			<Input
				label="Make"
				name="make"
				control={control}
				rules={{ required: 'Make is required' }}
			/>
			<Input
				label="Model"
				name="model"
				control={control}
				rules={{ required: 'Model is required' }}
			/>
			<Input
				label="Color"
				name="color"
				control={control}
				rules={{ required: 'Color is required' }}
			/>
			<div className="grid grid-cols-2 gap-3">
				<Input
					label="Year"
					name="year"
					type="number"
					control={control}
					rules={{ required: 'Year is required' }}
				/>
				<Input
					label="Mileage"
					name="mileage"
					type="number"
					control={control}
					rules={{ required: 'Mileage is required' }}
				/>
			</div>

			{isCreate && (
				<>
					<Input
						label="Image URL"
						name="imageUrl"
						control={control}
						rules={{ required: 'Image URL is required' }}
					/>
					<div className="grid grid-cols-2 gap-3">
						<Input
							label="Reserve Price (enter 0 if no reserve)"
							name="reservePrice"
							type="number"
							control={control}
							rules={{ required: 'Reserve Price is required' }}
						/>
						<DateInput
							label="Auction end date/time"
							name="auctionEnd"
							dateFormat="dd MMMM yyyy h:mm a"
							showTimeSelect
							control={control}
							rules={{ required: 'Auction end  date is required' }}
						/>
					</div>
				</>
			)}

			<div className="flex justify-between">
				<Button outline color="gray">
					Cancel
				</Button>
				<Button
					isProcessing={isSubmitting}
					disabled={!isValid}
					type="submit"
					outline
					color="success"
				>
					Submit
				</Button>
			</div>
		</form>
	);
}
