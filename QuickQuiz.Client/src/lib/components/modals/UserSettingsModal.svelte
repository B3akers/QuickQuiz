<script lang="ts">
    import { getContext } from "svelte";
    import { Modal, Toggle, Helper } from "flowbite-svelte";

    const settings: any = getContext("settings");

    let { isOpen = $bindable() as boolean } = $props();
    let streamerMode = $state($settings.streamerMode ?? false);

    $effect(() => {
        const settingsObj = {
            streamerMode,
        };

        $settings = settingsObj;

        window.localStorage.setItem(
            "userSettings",
            JSON.stringify(settingsObj),
        );
    });
</script>

<Modal
    size="xs"
    title="Twoje ustawienia"
    bind:open={isOpen}
    autoclose
    outsideclose
>
    <Toggle bind:checked={streamerMode}>Streamer mode</Toggle>
    <Helper class="select-none">
        Poprawna odpowiedź pokazuje się dopiero po upływie czasu.
    </Helper>
</Modal>
