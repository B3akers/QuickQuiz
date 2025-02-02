<script lang="ts">
	import { onMount, setContext } from "svelte";
	import { writable, type Writable } from "svelte/store";
	import MainForm from "./MainForm.svelte";
	import ErrorPageForm from "./ErrorPageForm.svelte";
	import { WebSocketManager } from "$lib/client/websocket";

	type StageNames = "MainForm" | "ErrorAnotherSession";
	type WsErros = "another_session" | "connection_failed";

	const websocket: Writable<WebSocketManager> = writable();
	setContext("websocket", websocket);

	let stage: StageNames = $state("MainForm");
	let errorAlert = $state();

	onMount(() => {
		websocket.set(new WebSocketManager());

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

{#if stage === "MainForm"}
	<MainForm />
{:else if stage === "ErrorAnotherSession"}
	<ErrorPageForm error={errorAlert} />
{/if}
