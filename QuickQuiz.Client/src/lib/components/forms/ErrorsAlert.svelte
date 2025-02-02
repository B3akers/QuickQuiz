<script lang="ts">
    import { Alert } from "flowbite-svelte";
    import { InfoCircleSolid } from "flowbite-svelte-icons";

    let { error } = $props();
</script>

{#if error}
    <Alert
        class="!items-start dark:bg-gray-700 mt-2 mb-2 text-left"
        rounded={true}
        color="red"
    >
        <span slot="icon">
            <InfoCircleSolid class="h-5 w-5" />
        </span>
        <p class="font-bold">
            Wystąpił błąd:
            <small class="font-normal">
                {#if typeof error === "string" || error instanceof String}
                    {error}
                {/if}
            </small>
        </p>
    </Alert>
{/if}
