import ShowLogin from '@/app/components/ShowLogin';
import React from 'react';

export default function SignIn({
	searchParams
}: {
	searchParams: { callbackUrl: string };
}) {
	return <ShowLogin callbackUrl={searchParams.callbackUrl} />;
}
