<script lang="ts">
    import CategorySelection from "./CategorySelection.svelte";
    import PrepareForQuestion from "./PrepareForQuestion.svelte";
    import QuestionAnswering from "./QuestionAnswering.svelte";
    import { getContext } from "svelte";
    import { type Writable } from "svelte/store";

    const game: Writable<any> = getContext("gameState");
</script>

<main class="flex-grow flex justify-center items-center">
    {#if $game.stateId === "CategorySelection"}
        <CategorySelection />
    {:else if $game.stateId === "PrepareForQuestion"}
        <PrepareForQuestion />
    {:else if $game.stateId === "QuestionAnswering"}
        <QuestionAnswering />
    {/if}
</main>
