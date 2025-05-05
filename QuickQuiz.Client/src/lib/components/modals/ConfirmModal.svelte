<script lang="ts">
    import { Modal, Button } from "flowbite-svelte";
    import { ExclamationCircleOutline } from "flowbite-svelte-icons";

    let { isOpen = $bindable() as boolean, callback } = $props();
</script>

<Modal bind:open={isOpen} size="xs" autoclose outsideclose>
    <div class="text-center">
        <ExclamationCircleOutline
            class="mx-auto mb-4 h-12 w-12 text-gray-400 dark:text-gray-200"
        />
        <h3 class="mb-5 text-lg font-normal text-gray-500 dark:text-gray-400">
            Czy na pewno chcesz wykonać tą akcję?
        </h3>
        <Button color="red" class="me-2" onclick={callback}>Tak, jestem pewien</Button>
        <Button color="alternative">Nie, anuluj</Button>
    </div>
</Modal>