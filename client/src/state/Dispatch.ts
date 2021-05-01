
// import type {Page} from "./Page";
import type {PageState} from "./model/PageState";
import type {PageDescriptor} from "./model/PageDescriptor";

// type CommandAction = {
//   kind: "Command";
//   name: string;
// }

// type AsyncAction = {
//   kind: "Async";
//   action: () => Promise<void>;
// }

type PageAction = {
  kind: "Page";
  descriptor: PageDescriptor;
}


type UpdateAction = {
  kind: "Update";
  update: (state: PageState) => PageState | null;
}

export type DispatchAction =
    // | CommandAction
    // | AsyncAction
    | PageAction
    | UpdateAction;


export type Dispatch = (action: DispatchAction) => void;


export const Dispatch = {
  // Command: (name: string) => ({
  //   kind: "Command" as const,
  //   name: name
  // }),

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
