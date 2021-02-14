import Markdown from "../../components/common/Markdown";
import Effect from "../../components/play/Effect";
import Option from "../../components/play/Option";

import type {Scene} from "../../model/read/Scene";


export default function Scene(scene: Scene) {
  // todo: return, continue.

  return (
    <>
      <Markdown>{scene.text}</Markdown>
      {scene.effects.map(effect => <Effect {...effect} />)}
      <hr class="w-75 mx-auto mt-4 mb-5" />
      <div class="options list-group">
        {scene.options.map((option, index) =>
          <Option key={index} {...option} />)}
      </div>
    </>
  );
}
