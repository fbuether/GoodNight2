

// serviceBase is defined in index.html
declare var serviceBase: string;

export default async function request(url: string): Promise<object> {
  let fullUrl = serviceBase + (url.startsWith("/") ? url : "/" + url);
  let request = await fetch(fullUrl);
  let json = await request.json();
  return json;
}
