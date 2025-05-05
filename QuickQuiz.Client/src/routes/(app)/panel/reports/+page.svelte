<script lang="ts">
    import { Card, Button, Heading } from "flowbite-svelte";

    import { ConfirmModal } from "$lib/components/modals";
    import { del } from "$lib/client/requests";

    let { data }: any = $props();

    let reports = $derived(data.reportsData.reports);
    let questionDescriptor = $derived(
        Object.fromEntries(
            data.reportsData.questions.map((entry: any) => [entry.id, entry]),
        ),
    );
    let categoriesDescriptor = $derived(
        Object.fromEntries(
            data.reportsData.categories.map((entry: any) => [entry.id, entry]),
        ),
    );

    let currentIndex = $state(0);
    let isDeclineConfirmOpen = $state(false);
    let isAcceptConfirmOpen = $state(false);

    const reasonDescriptor = [
        "Nieaktualne pytanie",
        "Zła kategoria",
        "Zła odpowiedź",
        "Inne",
    ];
</script>

<main
    class="flex-grow flex flex-col items-center justify-center space-y-4 my-5"
>
    {#if reports?.length > 0}
        <Card class="space-y-5">
            <div
                class="text-white text-center p-2 rounded-3xl min-w-48"
                style="background-color: {categoriesDescriptor[
                    reports[currentIndex].categoryId
                ]?.color ?? 'black'};"
            >
                {categoriesDescriptor[reports[currentIndex].categoryId]
                    ?.label ?? "???"}
            </div>
            <Heading tag="h4" class="text-center"
                >{questionDescriptor[reports[currentIndex].questionId]
                    .text}</Heading
            >
            {#if questionDescriptor[reports[currentIndex].questionId].image}
                <img
                    class="select-none object-contain min-h-[220px] max-h-[40vh]"
                    alt="quiz"
                    src={questionDescriptor[reports[currentIndex].questionId]
                        .image}
                />
            {/if}
            {#each questionDescriptor[reports[currentIndex].questionId].answers as answer, index}
                <Button
                    color={index ==
                    questionDescriptor[reports[currentIndex].questionId]
                        .correctAnswer
                        ? "green"
                        : "red"}
                    disabled>{answer}</Button
                >
            {/each}
            <Heading class="text-center" tag="h6"
                >Powód: {reasonDescriptor[reports[currentIndex].reason] ??
                    "Inne"}</Heading
            >
            <div class="flex justify-between">
                <Button
                    onclick={() => (isAcceptConfirmOpen = true)}
                    color="green">Zatwierdź</Button
                >
                <Button
                    onclick={() => (isDeclineConfirmOpen = true)}
                    color="red">Odrzuć</Button
                >
            </div>
        </Card>

        <span class="text-gray-300">
            Report {currentIndex + 1} na {reports.length}
        </span>

        <div class="flex items-center justify-center space-x-4 py-4">
            <button
                onclick={() => (currentIndex = Math.max(currentIndex - 1, 0))}
                disabled={currentIndex === 0}
                class="px-4 py-2 rounded bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed text-white hover:bg-gray-700"
            >
                ← Poprzednie
            </button>

            <button
                onclick={() =>
                    (currentIndex = Math.min(
                        currentIndex + 1,
                        reports.length - 1,
                    ))}
                disabled={currentIndex === reports.length - 1}
                class="px-4 py-2 rounded bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed text-white hover:bg-gray-700"
            >
                Następne →
            </button>
        </div>
    {:else}
        <Heading tag="h3" class="text-center"
            >Aktualnie nie ma żadnych zgłoszeń</Heading
        >
        <Button href="/panel">Powrót</Button>
    {/if}
</main>

<ConfirmModal
    label="Czy na pewno chcesz odrzucić to zgłoszenie?"
    bind:isOpen={isDeclineConfirmOpen}
    callback={async () => {
        await del(
            fetch,
            `/moderator/question-report/${reports[currentIndex].id}/discard`,
        );
        reports = reports.filter((_: any, i: number) => i !== currentIndex);
        currentIndex = Math.min(Math.max(currentIndex, 0), reports.length - 1);
    }}
/>

<ConfirmModal
    label="Czy na pewno chcesz zatwierdzić to zgłoszenie, pytanie zostanie usunięte?"
    bind:isOpen={isAcceptConfirmOpen}
    callback={async () => {
        await del(
            fetch,
            `/moderator/question-report/${reports[currentIndex].id}/accept`,
        );
        reports = reports.filter((_: any, i: number) => i !== currentIndex);
        currentIndex = Math.min(Math.max(currentIndex, 0), reports.length - 1);
    }}
/>
