import type { PageLoad } from './$types';
import { browser } from '$app/environment';
import { get } from '$lib/client/requests';

export const prerender = true;

export const load: PageLoad = async ({ fetch }) => {
    if (browser) {
        return { statistics: await get(fetch, '/game/stats') };
    }
};