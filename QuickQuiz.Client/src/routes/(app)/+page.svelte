<script lang="ts">
	import { onMount, getContext } from "svelte";

	import MainForm from "./MainForm.svelte";
	import LobbyForm from "./LobbyForm.svelte";
	import ErrorPageForm from "./ErrorPageForm.svelte";
	import GameForm from "./GameForm.svelte";

	import { WebSocketManager } from "$lib/client/websocket";
	import { type Writable } from "svelte/store";
	import { Spinner } from "flowbite-svelte";

	let { data } = $props();

	type StageNames =
		| "Loading"
		| "LobbyForm"
		| "MainForm"
		| "GameForm"
		| "ErrorAnotherSession";
	type WsErros = "another_session" | "connection_failed";

	const session: any = getContext("session");
	const websocket: Writable<WebSocketManager> = getContext("websocket");
	const game: Writable<any> = getContext("gameState");

	let stage: StageNames = $state("Loading");
	let errorAlert = $state();

	function handleGameStatePacket(gameState: any) {
		game.set({
			lobby: gameState.lobby,
			categoryVote: gameState.categoryVote,
			stateId: gameState.stateId,
			prepareForQuestion: gameState.prepareForQuestion,
			gamePlayers: gameState.gamePlayers,
			questionAnswering: gameState.questionAnswering,
		});

		let nextStage: StageNames = "MainForm";

		if (gameState.lobby) {
			nextStage = "LobbyForm";
		}

		if (gameState.stateId != "None") {
			nextStage = "GameForm";
		}

		stage = nextStage;
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

	function handleLobbyActiveGameUpdate(data: any) {
		if (!$game?.lobby) return;

		$game.lobby.activeGameId = data.gameId;
	}

	function handleGameCategoryVote(data: any) {
		if (!$game?.categoryVote) return;

		$game.categoryVote.selectedCategory = data.categoryId;
	}

	function handleGameCategoryVoteStart(data: any) {
		$game.categoryVote = data.categoryVote;
		$game.stateId = "CategorySelection";
		stage = "GameForm";
	}

	function handleGamePrepareForQuestion(data: any) {
		$game.prepareForQuestion = data.prepareForQuestion;
		$game.stateId = "PrepareForQuestion";
		stage = "GameForm";
	}

	function handleGameQuestionAnswering(data: any) {
		$game.questionAnswering = data.questionAnswering;
		$game.stateId = "QuestionAnswering";
		stage = "GameForm";
	}

	function handleGamePlayers(data: any) {
		$game.gamePlayers = data.players;
	}

	function handleGameClearPlayerAnswers(data: any) {
		if (!$game?.gamePlayers) return;

		const values = Object.values($game.gamePlayers) as any[];
		for (let i = 0; i < values.length; i++) {
			values[i].roundAnswers.length = 0;
		}
	}

	function handleGameAnswerResult(data: any) {
		$game.questionAnswering.correctAnswerId = data.correctAnswerId;
		$game.questionAnswering.playerAnswers = data.playerAnswers;
		
		for (let i = 0; i < data.playerAnswers.length; i++) {
			const array = data.playerAnswers[i];
			if (!array) continue;
			for (let j = 0; j < array.length; j++) {
				$game.gamePlayers[array[j]].roundAnswers.push(data.correctAnswerId == i);
			}
		}
	}

	function handleGamePlayerAnswered(data: any) {
		const correctAnswerId = $game.questionAnswering.correctAnswerId;

		$game.questionAnswering.playerAnswers[data.answerId] = [
			...$game.questionAnswering.playerAnswers[data.answerId],
			data.playerId,
		];

		$game.gamePlayers[data.playerId].roundAnswers.push(
			correctAnswerId == data.answerId,
		);
	}

	function handleGameAnswerTimeout(data: any) {
		for(let i = 0; i < data.playerIds.length; i++) {
			$game.gamePlayers[data.playerIds[i]].roundAnswers.push(false);
		}
	}

	const packetHandlers: any = {
		gameState: handleGameStatePacket,
		lobbyPlayerJoin: handlePlayerLobbyJoinPacket,
		lobbyPlayerRemove: handlePlayerLobbyRemovePacket,
		lobbyTransferOwner: handlePlayerLobbyOwnerChangePacket,
		lobbyActiveGameUpdate: handleLobbyActiveGameUpdate,
		gameCategoryVote: handleGameCategoryVote,
		gameCategoryVoteStart: handleGameCategoryVoteStart,
		gamePrepareForQuestion: handleGamePrepareForQuestion,
		gamePlayers: handleGamePlayers,
		gameClearPlayerAnswers: handleGameClearPlayerAnswers,
		gameQuestionAnswering: handleGameQuestionAnswering,
		gameAnswerResult: handleGameAnswerResult,
		gamePlayerAnswered: handleGamePlayerAnswered,
		gameAnswerTimeout: handleGameAnswerTimeout
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
{:else if stage === "GameForm"}
	<GameForm />
{:else if stage === "MainForm"}
	<MainForm statistics={data.statistics} />
{:else if stage === "LobbyForm"}
	<LobbyForm />
{:else if stage === "ErrorAnotherSession"}
	<ErrorPageForm error={errorAlert} />
{/if}
