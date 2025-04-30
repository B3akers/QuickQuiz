<script lang="ts">
  import { Progressbar, Heading } from "flowbite-svelte";
  import { Tween } from "svelte/motion";
  import { linear } from "svelte/easing";
  import { onMount, getContext } from "svelte";

  const settings: any = getContext("settings");

  let { start, end } = $props();

  const rtf1 = new Intl.RelativeTimeFormat("pl", { style: "short" });

  const startMs = start.getTime();
  const endMs = end.getTime();
  const nowMs = Date.now();

  const totalDuration = Math.max(endMs - startMs, 0);

  const initialProgress =
    nowMs < startMs
      ? 0
      : nowMs >= endMs
        ? 100
        : ((nowMs - startMs) / totalDuration) * 100;
  const remainingDuration =
    nowMs < startMs ? totalDuration : nowMs >= endMs ? 0 : endMs - nowMs;

  function makeSeconds(value: number) {
    return Math.floor(value * 10) / 10;
  }

  const progress = new Tween(initialProgress, {
    duration: remainingDuration,
    easing: linear,
  });

  onMount(() => {
    progress.set(100);
  });
</script>

<div class="my-4">
  {#if !$settings.hideTimers}
    <Heading class="text-center select-none" color="text-orange-400" tag="h6"
      >{rtf1.format(
        makeSeconds(((100 - progress.current) / 100) * (totalDuration / 1000)),
        "seconds",
      )}</Heading
    >
    <Progressbar size="h-4" progress={100 - progress.current} />
  {/if}
</div>
