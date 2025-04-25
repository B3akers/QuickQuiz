<script lang="ts">
    import { Card, Heading, Button } from "flowbite-svelte";
    import { getContext } from "svelte";
    import { TimeProgressbar, AnswerButton } from "$lib/components";
    import { type Writable } from "svelte/store";

    const game: Writable<any> = getContext("gameState");

    function shuffle<T>(array: T[]) {
        let currentIndex = array.length;

        while (currentIndex != 0) {
            let randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex--;

            [array[currentIndex], array[randomIndex]] = [
                array[randomIndex],
                array[currentIndex],
            ];
        }
    }

    const currentQuestion = $derived.by(() => $game.questionAnswering.question);
    const answerButtons = $derived.by(() => {
        const indices = currentQuestion.answers.map((_: any, i: number) => i);
        shuffle(indices);
        return indices;
    });
</script>

<Card size="lg" class="space-y-5">
    <Heading tag="h4" class="text-center select-none"
        >{$game.questionAnswering.question.text}</Heading
    >
    <TimeProgressbar
        start={new Date($game.questionAnswering.startTime)}
        end={new Date($game.questionAnswering.endTime)}
    />
    {#if $game.questionAnswering.question.image}
        <img
            class="select-none object-contain min-h-[220px] max-h-[40vh]"
            alt="quiz"
            src={$game.questionAnswering.question.image}
        />
    {/if}
    {#each answerButtons as id}
        <AnswerButton
            disabled={$game.questionAnswering.correctAnswerId != null}
            answerId={id}
        />
    {/each}
</Card>
