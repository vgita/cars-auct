/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import { Button } from 'flowbite-react';
import { FieldValues, useForm } from 'react-hook-form';
import { useEffect } from 'react';
import Input from '../components/Input';
import DateInput from '../components/DateInput';
import { createAuction } from '../actions/auctionActions';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';

export default function AuctionForm() {
	const router = useRouter();
	const {
		control,
		handleSubmit,
		setFocus,
		formState: { isSubmitting, isValid }
	} = useForm({
		mode: 'onTouched'
	});

	useEffect(() => {
		setFocus('make');
	}, [setFocus]);

	async function onSubmit(data: FieldValues) {
		try {
			const res = await createAuction(data);

			if (res.error) {
				throw res.error;
			}

			router.push(`/auctions/details/${res.id}`);
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
