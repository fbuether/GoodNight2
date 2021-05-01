
// import type {Page} from "./Page";
import type {State} from "./State";
import type {PageState} from "./model/PageState";
import type {PageDescriptor} from "./model/PageDescriptor";

type CommandAction = {
  kind: "Command";
  action: () => void;
}

// type AsyncAction = {
//   kind: "Async";
//   action: () => Promise<void>;
// }

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
  update: (state: PageState) => PageState | null;
}

export type DispatchAction =
    | CommandAction
    | StateAction
   // | AsyncAction
    | PageAction
    | UpdateAction;


export type Dispatch = (action: DispatchAction) => void;


export const Dispatch = {
  Command: (action: () => void) => ({
    kind: "Command" as const,
    action: action
  }),

  State: (action: (state: State) => State) => ({
    kind: "State" as const,
    action: action
  }),

  // Async: (action: () => Promise<void>) => ({
  //   kind: "Async" as const,
  //   action: action
  // }),

  Page: (descriptor: PageDescriptor) => ({
    kind: "Page" as const,
    descriptor: descriptor
  }),

  Update: (update: (state: PageState) => PageState | null) => ({
    kind: "Update" as const,
    update: update
  })
}



export const messages = new Array<DispatchAction>();


var localExecutor = () => {};

export function setExecutor(executor: () => void) {
  localExecutor = executor;
}


export function dispatch(dispatch: DispatchAction) {
  messages.push(dispatch);
  setTimeout(() => localExecutor());
}
