
import type {State} from "../state/State";
import type {Page} from "../state/Page";
import type {Pages} from "../state/Pages";
import type {PageDescriptor} from "./PageDescriptor";
import {StateStore} from "./StateStore";
import {History} from "./History";


type CommandAction = {
  kind: "Command";
  action: () => void;
}

type StateAction = {
  kind: "State";
  action: (state: State) => State;
}

type PageAction = {
  kind: "Page";
  descriptor: PageDescriptor;
}

type UpdateAction = {
  kind: "Update";
  update: (state: Pages) => Pages | null;
}

export type DispatchAction =
    | CommandAction
    | StateAction
    | PageAction
    | UpdateAction;


export type Dispatch = (action: DispatchAction) => void;


// the store of messages that need to be processed.
const messages = new Array<DispatchAction>();


export function update() {
  if (messages.length == 0) {
    return;
  }

  let msg = messages.shift();
  if (msg === undefined) {
    return;
  }

  console.log("executenext: ", msg);

  switch (msg.kind) {
    case "Command":
      msg.action();
      break;

    case "State":
      StateStore.update(msg.action);
      break;

    case "Update":
      let upd = msg.update;
      StateStore.update(state => {
        var updated = upd(state.page);
        return updated != null
            ? { ...state, page: updated }
            : null;
      });
      break;

    case "Page":
      let desc = msg.descriptor;
      let newState = StateStore.update(state => ({ ...state, page: desc.state }));
      document.title = desc.title;

      // must be prior to history push, as UserService must read a first url
      // that we have been redirected to.
      if (desc.onLoad) {
        desc.onLoad(Dispatch.send, newState);
      }

      History.push(desc);
  }

  if (messages.length == 0) {
    onFinish();
  }
  else {
    update();
  }
}


// will be filled with the rendering function in index.tsx.
var onFinish = () => {};


export const Dispatch = {
  Command: (action: () => void) => ({
    kind: "Command" as const,
    action: action
  }),

  State: (action: (state: State) => State) => ({
    kind: "State" as const,
    action: action
  }),

  Page: (descriptor: PageDescriptor) => ({
    kind: "Page" as const,
    descriptor: descriptor
  }),

  Update: (update: (state: Pages) => Pages | null) => ({
    kind: "Update" as const,
    update: update
  }),


  // messages: messages,
  send: (dispatch: DispatchAction) => {
    messages.push(dispatch);
    setTimeout(() => update());
  },

  update: update,

  onFinishUpdate: (action: () => void) => {
    onFinish = action;
  }
}
