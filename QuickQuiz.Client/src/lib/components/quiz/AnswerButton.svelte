<script lang="ts">
    import { Heading, Badge } from "flowbite-svelte";
    import { getContext } from "svelte";
    import { type Writable } from "svelte/store";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { UserName } from "$lib/components";

    const { questionAnswer, gamePlayers, questionAnswering, stateId }: any =
        getContext("gameState");
    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const session: Writable<any> = getContext("session");
    const settings: Writable<any> = getContext("settings");

    let { answerId }: any = $props();

    const playerAnswers = $derived($questionAnswer?.playerAnswers[answerId]);
    const correctAnswerId = $derived($questionAnswer?.correctAnswerId);
    const isPlayerAnswer = $derived(
        $questionAnswer?.playerAnswers[answerId]?.includes($session.id),
    );
</script>

{#if correctAnswerId >= 0}
    {#if !$settings.streamerMode || $stateId == "QuestionAnswered"}
        <button
            disabled
            class="text-center font-medium inline-flex flex-col items-center justify-center px-5 py-2.5 text-sm text-white bg-gray-800 rounded-lg"
            class:border={correctAnswerId != answerId &&
                !(playerAnswers?.length > 0)}
            class:border-gray-600={correctAnswerId != answerId &&
                !(playerAnswers?.length > 0)}
            class:bg-red-600={correctAnswerId != answerId &&
                playerAnswers?.length > 0}
            class:bg-green-700={correctAnswerId == answerId}
            class:outline-none={isPlayerAnswer}
            class:ring-4={isPlayerAnswer}
            class:ring-red-900={isPlayerAnswer && correctAnswerId != answerId}
            class:ring-green-800={isPlayerAnswer && correctAnswerId == answerId}
        >
            <Heading tag="h6">
                {$questionAnswering.question.answers[answerId]}
            </Heading>
            <div class="flex flex-wrap justify-center gap-1 w-full">
                {#each playerAnswers as playerId}
                    <Badge color="dark"
                        ><UserName user={$gamePlayers[playerId]} /></Badge
                    >
                {/each}
            </div>
        </button>
    {:else}
        <button
            disabled
            class="text-center font-medium inline-flex flex-col items-center justify-center px-5 py-2.5 text-sm text-white bg-gray-800 rounded-lg"
            class:border={!isPlayerAnswer}
            class:border-gray-600={!isPlayerAnswer}
            class:outline-none={isPlayerAnswer}
            class:ring-4={isPlayerAnswer}
            class:ring-gray-900={isPlayerAnswer}
            ><Heading color={!isPlayerAnswer ? "text-gray-500" : ""} tag="h6">
                {$questionAnswering.question.answers[answerId]}
            </Heading></button
        >
    {/if}
{:else}
    <button
        onclick={() => {
            $websocket?.sendMessage({
                $type: "gameQuestionAnswer",
                questionId: $questionAnswering.question.id,
                answerId: answerId,
            });
        }}
        class="text-center font-medium focus-within:ring-4 focus-within:outline-none inline-flex items-center justify-center px-5 py-2.5 text-sm border bg-gray-800 text-white border-gray-600 hover:bg-gray-700 hover:border-gray-600 focus-within:ring-gray-700 rounded-lg"
        ><Heading tag="h6"
            >{$questionAnswering.question.answers[answerId]}</Heading
        ></button
    >
{/if}
