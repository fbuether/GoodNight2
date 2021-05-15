import type {Scene as State} from "../../model/read/Scene";

import Markdown from "../../components/common/Markdown";
import Effect from "../../components/read/Effect";
import Option from "../../components/read/Option";


export default function Scene(scene: State) {
  // todo: return, continue.

  return (
    <>
      <Markdown>{scene.text}</Markdown>
      {scene.effects.map(effect => <Effect {...effect} />)}
      <hr class="w-75 mx-auto mt-4 mb-5" />
      <div class="options list-group">
        {scene.options.map((option, index) => <Option {...option} />)}
      </div>
    </>
  );

}
