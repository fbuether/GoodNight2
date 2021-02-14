import Markdown from "../../components/common/Markdown";

import Option from "./Option";

import type {Scene} from "../../model/read/Scene";


export default function Scene(scene: Scene) {
  // todo: effects.

  // todo: return, continue.

  return (
    <>
      <Markdown text={scene.text} />
      <hr class="w-75 mx-auto mt-4 mb-5" />
      <div class="options list-group">
        {scene.options.map((option, index) =>
          <Option key={index} {...option} />)}
      </div>
    </>
  );
}
