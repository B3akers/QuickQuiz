<script lang="ts">
    import { Card, Heading } from "flowbite-svelte";
    import { onMount, getContext } from "svelte";
    import { TimeProgressbar, AnswerButton } from "$lib/components";
    import { QuestionReportModal } from "$lib/components/modals";

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

    let isOpenReportModal = $state(false);

    let cardContainer: HTMLElement;
    onMount(() => {
        cardContainer.scrollIntoView({
            behavior: "auto",
            block: "start",
        });
    });
</script>

<div bind:this={cardContainer}>
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
        <a
            href={null}
            onclick={() => (isOpenReportModal = true)}
            class="cursor-pointer text-center mx-auto">Zgłoś pytanie</a
        >
    </Card>

    <QuestionReportModal
        questionId={$questionAnswering.question.id}
        bind:isOpen={isOpenReportModal}
    />
</div>
