
export interface WithPath<TPage> {
  readonly path: RegExp;
  readonly ofUrl: (path: string, matches: Array<string>) => TPage;
}


export const OfUrl = {
  union<TPage, THandler extends WithPath<TPage>, TFallback extends TPage>(
    pathname: string | undefined,
    pages: Array<THandler>,
    fallback: TFallback): TPage {

    if (pathname === undefined) {
      return fallback;
    }

    let page = pages.find(p => p.path.test(pathname));
    if (page !== undefined) {
      let matches = pathname.match(page.path);
      if (matches == null) {
        return fallback;
      }

      return page.ofUrl(pathname, matches);
    }
    else {
      return fallback;
    }
  }
}
