import {JSX} from "preact";
import * as PreactHooks from "preact/hooks";

type TextAreaEvent = JSX.TargetedEvent<HTMLTextAreaElement, Event>;


interface TextareaState {
  class: string;
  onChange: (event: TextAreaEvent) => void;
  content: string | null;
}


function refitTextarea(textarea: HTMLTextAreaElement) {
  let initialCH = textarea.clientHeight;
  let initialStyle = textarea.style.height;
  textarea.style.height = "";
  let fitSH = textarea.scrollHeight;

  textarea.style.height = initialCH < fitSH
      ? (fitSH + 9).toString() + "px"
      : initialStyle;

  return initialCH < fitSH;
}

function updateDataWithCallback(callback: (event: TextAreaEvent) => void) {
  return (event: TextAreaEvent) => {
    let didRefit = refitTextarea(event.currentTarget);
    if (didRefit) {
      callback(event);
    }
  };
}


export default function ScalingTextarea(state: TextareaState) {
  let textarea = PreactHooks.useRef<HTMLTextAreaElement>(null);
  PreactHooks.useEffect(() => {
    refitTextarea(textarea.current);
  });

  return (
    <span class="scaling-textarea" data-value={state.content}>
      <textarea class={state.class} ref={textarea}
        onInput={updateDataWithCallback(state.onChange)}>{state.content}
      </textarea>
    </span>
  );
}
