<script lang="ts">
    import { ErrorsAlert } from "$lib/components";

    let { error } = $props();
</script>

<main class="flex-grow flex flex-col items-center justify-center space-y-4 text-lg">
    <ErrorsAlert {error} />
</main>
