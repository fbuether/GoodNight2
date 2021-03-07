
export function goTo(url: string, title: string = "GoodNight") {
  let lastUrl = history.state;
  if (lastUrl != url) {
    history.pushState(url, title, url);
  }
}
