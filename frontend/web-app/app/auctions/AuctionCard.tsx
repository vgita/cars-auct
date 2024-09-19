import { Auction } from '@/types';
import CarImage from './CarImage';
import CountdownTimer from './CountdownTimer';

type Props = {
	auction: Auction;
};

export default function AuctionCard({ auction }: Props) {
	return (
		<a href="#" className="group">
			<div
				className="
        relative
        w-full
        bg-gray-200
        aspect-video
        rounded-lg
        overflow-hidden"
			>
				<CarImage
					imageUrl={auction.imageUrl}
					make={auction.make}
					model={auction.model}
					color={auction.color}
				/>
				<div className="absolute bottom-2 left-2">
					<CountdownTimer auctionEnd={auction.auctionEnd} />
				</div>
			</div>
			<div
				className="
      flex
      justify-between
      items-center
      mt-4"
			>
				<h3 className="text-gray-700">
					{auction.make} {auction.model}
				</h3>
				<p className="font-semibold text-sm">{auction.year}</p>
			</div>
		</a>
	);
}