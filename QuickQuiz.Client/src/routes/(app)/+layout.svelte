<script lang="ts">
	import "../../app.css";
	import {
		Spinner,
		Footer,
		FooterCopyright,
		FooterIcon,
	} from "flowbite-svelte";
	import { GithubSolid } from "flowbite-svelte-icons";
	import { navigating } from "$app/state";
	import { onMount } from "svelte";
	import { setContext } from "svelte";
	import { writable, type Writable } from "svelte/store";
	import type { WebSocketManager } from "$lib/client/websocket";

	let { data, children } = $props();

	const websocket: Writable<WebSocketManager> = writable();
	const session: Writable<any> = writable(data.session);
	const settings: Writable<any> = writable({});

	setContext("session", session);
	setContext("websocket", websocket);
	setContext("settings", settings);
	setContext("gameState", {
		lobby: writable({}),
		lobbyGameSettings: writable({}),
		categoryVote: writable({}),
		stateId: writable("None"),
		prepareForQuestion: writable({}),
		gamePlayers: writable({}),
		questionAnswering: writable({}),
		questionAnswer: writable({}),
	});

	$effect(() => {
		session.set(data.session);
	});

	let isLoaded = $state(false);
	onMount(() => {
		try {
			$settings = JSON.parse(
				window.localStorage.getItem("userSettings") ?? "{}",
			);
		} catch {}
		isLoaded = true;
	});
</script>

{#if isLoaded}
	<div
		class={navigating.to
			? "opacity-30 select-none pointer-events-none"
			: ""}
	>
		<div class="flex flex-col min-h-screen">
			{@render children()}
			<Footer>
				<div
					class="py-6 px-4 bg-gray-700 flex items-center justify-center"
				>
					<FooterCopyright
						spanClass="text-sm text-gray-300 sm:text-center"
						by="QuickQuiz™"
					/>
					<div
						class="flex space-x-6 rtl:space-x-reverse sm:justify-center mt-0 ml-2"
					>
						<FooterIcon
							target="_blank"
							href="https://github.com/B3akers/QuickQuiz/"
						>
							<GithubSolid
								class="w-5 h-5 text-gray-500 dark:text-gray-500 hover:text-gray-900 dark:hover:text-white"
							/>
						</FooterIcon>
					</div>
				</div>
			</Footer>
		</div>
	</div>
{/if}

{#if navigating.to || !isLoaded}
	<div
		class="absolute top-0 left-0 w-full h-full flex z-10 opacity-100 justify-center items-center"
	>
		<Spinner size="10" class="opacity-90" />
	</div>
{/if}
