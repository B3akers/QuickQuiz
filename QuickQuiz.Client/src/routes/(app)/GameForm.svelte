<script lang="ts">
    import CategorySelection from "./CategorySelection.svelte";
    import PrepareForQuestion from "./PrepareForQuestion.svelte";
    import QuestionAnswering from "./QuestionAnswering.svelte";
    import { getContext } from "svelte";

    const { stateId }: any = getContext("gameState");

</script>

<main class="flex-grow flex justify-center items-center">
    {#if $stateId === "CategorySelection"}
        <CategorySelection />
    {:else if $stateId === "PrepareForQuestion"}
        <PrepareForQuestion />
    {:else if $stateId === "QuestionAnswering" || $stateId === "QuestionAnswered"}
        <QuestionAnswering />
    {/if}
</main>
