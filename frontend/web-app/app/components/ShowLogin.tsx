'use client';

import Heading from './Heading';
import { Button } from 'flowbite-react';
import { signIn } from 'next-auth/react';

type Props = {
	title?: string;
	subtitle?: string;
	showLogin?: boolean;
	callbackUrl?: string;
};

export default function ShowLogin({
	title = 'Should login to continue',
	subtitle = 'Choose a login method',
	showLogin = true,
	callbackUrl = '/'
}: Props) {
	return (
		<div className="h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg">
			<Heading title={title} subtitle={subtitle} center />
			<div className="mt-4">
				{showLogin && (
					<Button outline onClick={() => signIn('id-server', { callbackUrl })}>
						Login
					</Button>
				)}
			</div>
		</div>
	);
}
