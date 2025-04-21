<script lang="ts">
	import { onMount, getContext } from "svelte";
	import MainForm from "./MainForm.svelte";
	import LobbyForm from "./LobbyForm.svelte";
	import ErrorPageForm from "./ErrorPageForm.svelte";
	import { WebSocketManager } from "$lib/client/websocket";
	import { type Writable } from "svelte/store";
	import { Spinner } from "flowbite-svelte";

	type StageNames =
		| "Loading"
		| "LobbyForm"
		| "MainForm"
		| "ErrorAnotherSession";
	type WsErros = "another_session" | "connection_failed";

	const session: any = getContext("session");
	const websocket: Writable<WebSocketManager> = getContext("websocket");
	const game: Writable<any> = getContext("gameState");

	let stage: StageNames = $state("Loading");
	let errorAlert = $state();

	function handleGameStatePacket(gameState: any) {
		if (!gameState.lobby) {
			stage = "MainForm";
			return;
		}

		$game.lobby = gameState.lobby;

		stage = "LobbyForm";
	}

	function handlePlayerLobbyRemovePacket(data: any) {
		if (!$game?.lobby?.players) return;

		$game.lobby.players = $game.lobby.players.filter(
			(x: any) => x.id != data.playerId,
		);

		if (data.playerId === $session.id) {
			if (stage === "LobbyForm") stage = "MainForm";
			return;
		}
	}

	function handlePlayerLobbyJoinPacket(data: any) {
		if (!$game?.lobby?.players) return;

		$game.lobby.players = [...$game.lobby.players, data.player];
	}

	function handlePlayerLobbyOwnerChangePacket(data: any) {
		if (!$game?.lobby) return;

		$game.lobby.ownerId = data.playerId;
	}

	const packetHandlers: any = {
		gameState: handleGameStatePacket,
		lobbyPlayerJoin: handlePlayerLobbyJoinPacket,
		lobbyPlayerRemove: handlePlayerLobbyRemovePacket,
		lobbyTransferOwner: handlePlayerLobbyOwnerChangePacket
	};

	onMount(() => {
		if (!$websocket) websocket.set(new WebSocketManager());
		else $websocket.init();

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

				console.log(data);

				const packetType: string = data["$type"];
				const handler = packetHandlers[packetType];
				if (handler) handler(data);
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
{:else if stage === "LobbyForm"}
	<LobbyForm />
{:else if stage === "ErrorAnotherSession"}
	<ErrorPageForm error={errorAlert} />
{/if}
