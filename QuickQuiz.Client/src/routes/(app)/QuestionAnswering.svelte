<script lang="ts">
    import { Card, Heading } from "flowbite-svelte";
    import { getContext } from "svelte";
    import { TimeProgressbar, AnswerButton } from "$lib/components";

    const { questionAnswering }: any = getContext("gameState");

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

        return array;
    }
</script>

<Card size="lg" class="space-y-5">
    <Heading tag="h4" class="text-center select-none"
        >{$questionAnswering.question.text}</Heading
    >
    <TimeProgressbar
        start={new Date($questionAnswering.startTime)}
        end={new Date($questionAnswering.endTime)}
    />
    {#if $questionAnswering.question.image}
        <img
            class="select-none object-contain min-h-[220px] max-h-[40vh]"
            alt="quiz"
            src={$questionAnswering.question.image}
        />
    {/if}
    {#each shuffle($questionAnswering.question.answers.map((_: any, i: number) => i)) as id}
        <AnswerButton answerId={id} />
    {/each}
</Card>
