import * as PreactHooks from "preact/hooks";

export default function useAsyncEffect(effect: () => Promise<void>) {
  PreactHooks.useEffect(() => {
    (async () => {
      await effect();
    })();
  });
}
