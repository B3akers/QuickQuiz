<script lang="ts">
	import { onMount, getContext } from "svelte";

	import MainForm from "./MainForm.svelte";
	import LobbyForm from "./LobbyForm.svelte";
	import ErrorPageForm from "./ErrorPageForm.svelte";
	import GameForm from "./GameForm.svelte";
	import LeaderboardForm from "./LeaderboardForm.svelte";

	import { WebSocketManager } from "$lib/client/websocket";
	import { type Writable } from "svelte/store";
	import { Spinner } from "flowbite-svelte";

	type StageNames =
		| "Loading"
		| "LobbyForm"
		| "MainForm"
		| "GameForm"
		| "LeaderboardForm"
		| "ErrorAnotherSession";
	type WsErros = "another_session" | "connection_failed";

	const session: any = getContext("session");
	const websocket: Writable<WebSocketManager> = getContext("websocket");
	const {
		lobby,
		lobbyGameSettings,
		categoryVote,
		stateId,
		prepareForQuestion,
		gamePlayers,
		questionAnswering,
		questionAnswer,
	}: any = getContext("gameState");

	let stage: StageNames = $state("Loading");
	let errorAlert = $state();

	function handleGameStatePacket(gameState: any) {
		$lobby = gameState.lobby;
		$categoryVote = gameState.categoryVote;
		$stateId = gameState.stateId;
		$prepareForQuestion = gameState.prepareForQuestion;
		$gamePlayers = gameState.gamePlayers;
		$questionAnswering = gameState.questionAnswering;
		$questionAnswer = gameState.questionAnswer;
		$lobbyGameSettings = gameState.lobbyGameSettings;

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
		if (!$lobby?.players) return;

		$lobby.players = $lobby.players.filter(
			(x: any) => x.id != data.playerId,
		);

		if (data.playerId === $session.id) {
			if (stage === "LobbyForm") stage = "MainForm";
			return;
		}
	}

	function handlePlayerLobbyJoinPacket(data: any) {
		if (!$lobby?.players) return;

		$lobby.players = [...$lobby.players, data.player];
	}

	function handlePlayerLobbyOwnerChangePacket(data: any) {
		if (!$lobby) return;

		$lobby.ownerId = data.playerId;
	}

	function handleLobbyActiveGameUpdate(data: any) {
		if (!$lobby) return;

		$lobby.activeGameId = data.gameId;
	}

	function handleGameCategoryVote(data: any) {
		if (!$categoryVote) return;

		$categoryVote.selectedCategory = data.categoryId;
	}

	function handleGameCategoryVoteStart(data: any) {
		$categoryVote = data.categoryVote;
		$stateId = "CategorySelection";
		stage = "GameForm";
	}

	function handleGamePrepareForQuestion(data: any) {
		$prepareForQuestion = data.prepareForQuestion;
		$stateId = "PrepareForQuestion";
		stage = "GameForm";
	}

	function handleGameQuestionAnswering(data: any) {
		$questionAnswering = data.questionAnswering;
		$questionAnswer = {};
		$stateId = "QuestionAnswering";
		stage = "GameForm";
	}

	function handleGamePlayers(data: any) {
		$gamePlayers = data.players;
	}

	function handleGameClearPlayerAnswers(data: any) {
		if (!$gamePlayers) return;

		const values = Object.values($gamePlayers) as any[];
		for (let i = 0; i < values.length; i++) {
			values[i].roundAnswers.length = 0;
		}
	}

	function handleGameAnswerResult(data: any) {
		$questionAnswer = data.questionAnswer;

		for (let i = 0; i < data.questionAnswer.playerAnswers.length; i++) {
			const array = data.questionAnswer.playerAnswers[i];
			if (!array) continue;
			for (let j = 0; j < array.length; j++) {
				$gamePlayers[array[j]].roundAnswers.push(
					data.questionAnswer.correctAnswerId == i,
				);
			}
		}
	}

	function handleGamePlayerAnswered(data: any) {
		const correctAnswerId = $questionAnswer.correctAnswerId;
		if (!$questionAnswer.playerAnswers[data.answerId]) {
			$questionAnswer.playerAnswers[data.answerId] = [data.playerId];
		} else {
			$questionAnswer.playerAnswers[data.answerId] = [
				...$questionAnswer.playerAnswers[data.answerId],
				data.playerId,
			];
		}

		$gamePlayers[data.playerId].roundAnswers.push(
			correctAnswerId == data.answerId,
		);
	}

	function handleGameAnswerTimeout(data: any) {
		if (data.playerIds) {
			for (let i = 0; i < data.playerIds.length; i++) {
				$gamePlayers[data.playerIds[i]].roundAnswers.push(false);
			}
		}

		$stateId = "QuestionAnswered";
	}

	function handleGameFinished(data: any) {
		for (let i = 0; i < data.playerPoints.length; i++) {
			const keyValue = data.playerPoints[i];
			if (!$gamePlayers[keyValue.key]) continue;
			$gamePlayers[keyValue.key].points = keyValue.value;
		}
		stage = "LeaderboardForm";
	}

	function handleLobbyUpdateSettings(data: any) {
		$lobby.settings = data.settings;
	}

	function handleLobbyGameUpdateSettings(data: any) {
		$lobbyGameSettings = data.settings;
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
		gameAnswerTimeout: handleGameAnswerTimeout,
		gameFinished: handleGameFinished,
		lobbyUpdateSettings: handleLobbyUpdateSettings,
		lobbyGameUpdateSettings: handleLobbyGameUpdateSettings,
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
{:else if stage === "LeaderboardForm"}
	<LeaderboardForm />
{:else if stage === "MainForm"}
	<MainForm />
{:else if stage === "LobbyForm"}
	<LobbyForm />
{:else if stage === "ErrorAnotherSession"}
	<ErrorPageForm error={errorAlert} />
{/if}
