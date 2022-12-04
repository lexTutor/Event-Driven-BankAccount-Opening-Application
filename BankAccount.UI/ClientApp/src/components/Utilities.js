export const postData = async (url = '', data = {}) => {
    const response = await fetch(url, {
        method: 'POST',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });

    console.log(response);
    return response.json();
}

