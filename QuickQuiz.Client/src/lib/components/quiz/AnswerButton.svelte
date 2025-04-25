<script lang="ts">
    import { Heading, Badge } from "flowbite-svelte";
    import { getContext } from "svelte";
    import { type Writable } from "svelte/store";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { UserName } from "$lib/components";

    const game: Writable<any> = getContext("gameState");
    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const session: Writable<any> = getContext("session");

    let { answerId, disabled }: any = $props();

    let hasAnyAnswer = $derived.by(() => {
        return $game.questionAnswering.playerAnswers[answerId]?.length > 0;
    });

    let correct = $derived.by(() => {
        return $game.questionAnswering.correctAnswerId == answerId;
    });

    let active = $derived.by(() => {
        return $game.questionAnswering.playerAnswers[answerId]?.includes(
            $session.id,
        );
    });
</script>

{#if disabled}
    <button
        disabled
        class="text-center font-medium inline-flex flex-col items-center justify-center px-5 py-2.5 text-sm text-white bg-gray-800 rounded-lg"
        class:border={!correct && !hasAnyAnswer}
        class:border-gray-600={!correct && !hasAnyAnswer}
        class:bg-red-600={!correct && hasAnyAnswer}
        class:bg-green-700={correct}
        class:outline-none={active}
        class:ring-4={active}
        class:ring-red-900={active && !correct}
        class:ring-green-800={active && correct}
    >
        <Heading tag="h6">
            {$game.questionAnswering.question.answers[answerId]}
        </Heading>
        <div class="flex flex-wrap justify-center gap-1 w-full">
            {#each $game.questionAnswering.playerAnswers[answerId] as playerId}
                <Badge color="dark"
                    ><UserName user={$game.gamePlayers[playerId]} /></Badge
                >
            {/each}
        </div>
    </button>
{:else}
    <button
        onclick={() => {
            $websocket?.sendMessage({
                $type: "gameQuestionAnswer",
                questionId: $game.questionAnswering.question.id,
                answerId: answerId,
            });
        }}
        class="text-center font-medium focus-within:ring-4 focus-within:outline-none inline-flex items-center justify-center px-5 py-2.5 text-sm border bg-gray-800 text-white border-gray-600 hover:bg-gray-700 hover:border-gray-600 focus-within:ring-gray-700 rounded-lg"
        ><Heading tag="h6"
            >{$game.questionAnswering.question.answers[answerId]}</Heading
        ></button
    >
{/if}
