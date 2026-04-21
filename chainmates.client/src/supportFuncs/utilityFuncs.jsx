export default { getRandomItem, contactAPI, getArrayObjByID }



export async function contactAPI(urlTarget, method, authorized = false, bodyDict = {}, onFailReturn = null) {
    // I use this function exclusively to handle fetches to the database. Standardises
    // the urlStub, authorisation, and headers etc; optionally controls the return field
    // on a failed call.
    const urlStub = "/chainmates/"
    let credentials = authorized ? "include" : "same-origin";
    let fetchData;
    switch (method) {
        case "get":
            fetchData = {
                method: method,
                credentials: credentials
            }
            break;
        default:
            fetchData = {
                method: method,
                credentials: credentials,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(bodyDict)
            }
    }

    let response = await fetch(`${urlStub}${urlTarget}`,
        fetchData
    )

    if ((method == "get" && response.status != 200) || !response.ok) {
        console.log("HTTP Error ", response.status, "at url ", `${urlStub}${urlTarget}`);
        return onFailReturn ?? {};
    }
    else {
        console.log("SUCCESS at url ", `${urlStub}${urlTarget}`);
    }
    return response.json();
};


export function getArrayObjByID(array, id) {
    // Getting an object from a list of like objects by its (assumed present) id field
    const idMatch = (obj) => obj.id == id;
    return array.find(idMatch);
}

export function getRandomItem(array, numberOfResults = 1, arrayOfOne = false) {
    // Handles getting n objects from an array of length k, including edge cases 
    // of k < n, or only one option available (optionally put into an array of 
    // one).


    if ((numberOfResults == 1 || array.length == 1) && !(arrayOfOne)) {
        return array[Math.floor(Math.random() * array.length)];
    }

    let set = new Set();
    while (set.size < numberOfResults && set.size < array.length) {
        var randomIndex = Math.floor(Math.random() * array.length);
        set.add(randomIndex);
    }
    let randomIndexArray = Array.from(set);

    return randomIndexArray.map(index => array[index]);
}