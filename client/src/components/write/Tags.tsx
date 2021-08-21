import {Dispatch} from "../../core/Dispatch";

import type {Story} from "../../state/model/write/Story";
import {WriteStory} from "../../state/page/write/WriteStory";

import Link from "../common/Link";


function Tag(story: Story, tag: string) {
  let showTag = Dispatch.Page(WriteStory.page(story, tag));

  return (
    <li class="list-inline-item">
      <Link action={showTag}>
        {tag}
      </Link>
    </li>
  );
}


export default function Tags(story: Story, tags: Array<string>) {

  if (tags.length > 0) {
    return (
      <ul class="tags list-unstyled list-inline">
        {tags.map(tag => Tag(story, tag))}
      </ul>
    );
  }
  else {
    return <></>;
  }
}
