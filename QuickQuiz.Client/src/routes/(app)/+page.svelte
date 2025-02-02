<script lang="ts">
	import { onMount, getContext } from "svelte";
	import MainForm from "./MainForm.svelte";
	import ErrorPageForm from "./ErrorPageForm.svelte";
	import { WebSocketManager } from "$lib/client/websocket";
	import { type Writable } from "svelte/store";
	import { Spinner } from "flowbite-svelte";

	type StageNames = "Loading" | "MainForm" | "ErrorAnotherSession";
	type WsErros = "another_session" | "connection_failed";

	const websocket: Writable<WebSocketManager> = getContext("websocket");

	let stage: StageNames = $state("Loading");
	let errorAlert = $state();

	onMount(() => {
		if (!$websocket) websocket.set(new WebSocketManager());
		else $websocket.init();

		$websocket.sendMessage({
			$type: "gameState",
		});

		let unsubscribe: any = undefined;
		const unsubscribeSocket = websocket.subscribe((value) => {
			if (!value) return;
			if (unsubscribe) unsubscribe();
			unsubscribe = value.subscribe((data) => {
				if (data.error) {
					const wsError: WsErros = data.error;
					const descriptor = {
						another_session:
							"Wykryliśmy połaczenie z innego urządzenia",
						connection_failed:
							"Nie udało się połączyć z serwerem gry",
					};

					errorAlert = descriptor[wsError] ?? "Nieznany błąd";
					stage = "ErrorAnotherSession";
					return;
				}


			});
		});

		return () => {
			unsubscribeSocket();
			if (unsubscribe) unsubscribe();
			if ($websocket) $websocket.disconnect();
		};
	});
</script>

{#if stage === "Loading"}
	<main class="flex-grow flex flex-col items-center justify-center space-y-4">
		<Spinner size="14" />
	</main>
{:else if stage === "MainForm"}
	<MainForm />
{:else if stage === "ErrorAnotherSession"}
	<ErrorPageForm error={errorAlert} />
{/if}
