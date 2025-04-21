import type { LayoutLoad } from './$types';
import { browser } from '$app/environment';
import { get } from '$lib/client/requests';
import { redirect } from '@sveltejs/kit';

export const prerender = true;

export const load: LayoutLoad = async ({ fetch, url }) => {
    if (browser) {
        let session = undefined;

        try {
            session = await get(fetch, '/user/session');
        } catch { }

        if (session) {
            if (url.pathname === '/login') {
                throw redirect(302, "/?" + url.searchParams.toString());
            }
        } else {
            if (url.pathname !== '/login') {
                throw redirect(302, "/login?" + url.searchParams.toString());
            }
        }

        return { session: session };
    }
};