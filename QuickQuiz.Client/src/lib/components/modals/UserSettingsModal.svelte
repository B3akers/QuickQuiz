<script lang="ts">
    import { getContext } from "svelte";
    import { Modal, Toggle, Helper } from "flowbite-svelte";

    const settings: any = getContext("settings");

    let { isOpen = $bindable() as boolean } = $props();
    let streamerMode = $state($settings.streamerMode ?? false);
    let hideTimers = $state($settings.hideTimers ?? false);

    $effect(() => {
        const settingsObj = {
            streamerMode,
            hideTimers,
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

    <Toggle bind:checked={hideTimers}>Hide timers</Toggle>
    <Helper class="select-none">
        Pozwala skupić się na pytaniu poprzez ukrycie wszystkich timerów.
    </Helper>
</Modal>
