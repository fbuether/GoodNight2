import type {Scene} from "./Scene";
import type {Quality} from "./Quality";


export interface Category {
  name: string;
  categories: Array<Category>;
  scenes: Array<Scene>;
  qualities: Array<Quality>;
}


export interface Story {
  name: string;
  urlname: string;
  icon: string | null;
  description: string;
}

