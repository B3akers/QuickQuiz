<script lang="ts">
    import { getContext } from "svelte";
    import { Modal, Button, Radio } from "flowbite-svelte";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const websocket: Writable<WebSocketManager> = getContext("websocket");

    let { isOpen = $bindable() as boolean, questionId } = $props();

    let reportReason = $state("0");
</script>

<Modal
    size="xs"
    title="Zgłaszanie pytania"
    bind:open={isOpen}
    outsideclose
    autoclose
>
    <ul
        class="w-full bg-white rounded-lg border border-gray-200 dark:bg-gray-800 dark:border-gray-600 divide-y divide-gray-200 dark:divide-gray-600"
    >
        <li>
            <Radio class="p-3" bind:group={reportReason} value="0"
                >Nieaktualne pytanie</Radio
            >
        </li>
        <li>
            <Radio class="p-3" bind:group={reportReason} value="1"
                >Zła kategoria</Radio
            >
        </li>
        <li>
            <Radio class="p-3" bind:group={reportReason} value="2"
                >Zła odpowiedź</Radio
            >
        </li>
        <li>
            <Radio class="p-3" bind:group={reportReason} value="3">Inne</Radio>
        </li>
    </ul>
    <Button
        class="w-full"
        onclick={() => {
            $websocket?.sendMessage({
                $type: "reportQuestion",
                questionId: questionId,
                reason: parseInt(reportReason),
            });
        }}>Wyślij</Button
    >
</Modal>
