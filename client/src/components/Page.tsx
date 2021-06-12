import {Home} from "./Home";
import {FinishSignIn} from "./user/FinishSignIn";
import {RequireSignIn} from "./user/RequireSignIn";
import {StoryOverview} from "./read/StoryOverview";
import {ReadStory} from "./read/ReadStory";
import {SelectStory} from "./write/SelectStory";
import {CreateStory} from "./write/CreateStory";
import {WriteStory} from "./write/WriteStory";
import {WriteScene} from "./write/WriteScene";
import {WriteQuality} from "./write/WriteQuality";
import {StartAdventure} from "./read/StartAdventure";

import {Navigation} from "./navigation/Navigation";

import type {Pages} from "../state/Pages";
import type {Page as State} from "../state/Page";


// from webpack.config.js with DefinePlugin.
declare var _git_revision_version: string;
declare var _git_revision_commithash: string;


const othftwy = require("../../assets/othftwy.gif");




function renderPage(page: Pages) {
  switch (page.page) {
    case "ReadStory": return ReadStory(page);
    case "StartAdventure": return StartAdventure(page);
    case "StoryOverview": return StoryOverview(page);

    case "FinishSignIn": return FinishSignIn(page);
    case "RequireSignIn": return RequireSignIn(page);

    case "CreateStory": return CreateStory(page);
    case "SelectStory": return SelectStory(page);
    case "WriteQuality": return WriteQuality(page);
    case "WriteScene": return WriteScene(page);
    case "WriteStory": return WriteStory(page);

    case "Home": return Home(page);
  }
}


export default function Page(state: State) {
  return (
    <div id="page"
      class="container-lg shadow-around mt-lg-4 px-4 pt-lg-1">
        <Navigation page={state.page.page} user={state.user} />
      <hr class="mt-0" />
      {renderPage(state.page)}
      <footer id="schlussvermerk">
        <hr class="decorated w-75" />
        GoodNight {_git_revision_version}, build {_git_revision_commithash} ~{" "}
        <a href="https://jasminefields.net/" title="jasminefields.net">jasminefields.net</a> ~{" "}
        <a href="https://discord.gg/qwrzStrDwA" target="_blank">discord</a> ~{" "}
        <a href="https://github.com/fbuether/GoodNight2">github</a> ~{" "}
        <img src={othftwy} title="on the hunt for the white yonder" alt="hunt" />
      </footer>
    </div>
  );
}
