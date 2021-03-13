import * as PreactHooks from "preact/hooks";

import DispatchContext from "../../DispatchContext";
import useAsyncEffect from "../../ui/useAsyncEffect";

import {Story} from "../../model/read/Story";

import {State} from "../../state/State";
import {SelectStoryPart as PartState} from "../../state/read/SelectStoryPart";


export default function SelectStory(part: PartState) {
  const dispatch = PreactHooks.useContext(DispatchContext);

  if (part.stories == null) {
    useAsyncEffect(async() => {
      let stories: Array<Story> | null = [
        {name:"abc",urlname:"abc"},
        {name:"abc2",urlname:"abc2"}
      ];

      dispatch(State.lens.page.read.part.selectStory.stories.set(stories));
    });

    return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
          Loading...
      </div>
      <div id="side" class="col-sm-4"></div>
    </div>
    );
  }
  else {
    console.log(part);

    let stories = part.stories.map(story => story.urlname);

    return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        {stories.map(s => <span>{s}</span>)}
      </div>
      <div id="side" class="col-sm-4"></div>
    </div>
    );
  }
}

// }
// else {
//   let adventure = page.adventure;

//   return (
//     <div id="centre" class="row px-0 g-0">
//       <div id="text" class="col-sm-8">
//         <h1 id="banner">{adventure.story.name}</h1>
//         <Log entries={adventure.history}></Log>
//         <Scene {...adventure.current}></Scene>
//       </div>
//       <div id="side" class="col-sm-4">
//         <hr class="w-75 mx-auto mt-4 mb-5" />
//         <State {...adventure.player}></State>
//       </div>
//     </div>
//   );
// }
