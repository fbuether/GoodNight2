import * as React from "react";

import {Update} from "./update/Update";
import State from "./model/State";

// Create an empty dispatcher until we fill in the actual proper dispatcher,
// which happens in index.tsx.
const initial: React.Dispatch<
    React.ReducerAction<React.Reducer<State, Update>>> = (a) => {};

export default React.createContext(initial);

