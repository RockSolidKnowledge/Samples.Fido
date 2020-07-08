import {create, get} from "@github/webauthn-json";
import {PublicKeyCredentialDescriptorJSON} from "@github/webauthn-json/dist/src/json";
import {Base64urlString} from "@github/webauthn-json/src/base64url";
import {ajax} from "jquery";

export async function login(base64Challenge: string, relyingPartyId: string, base64KeyIds: string, completeLoginCallback: string) {

    let keys: Base64urlString[] = JSON.parse(base64KeyIds);
    let allowCredentials = Array<PublicKeyCredentialDescriptorJSON>();

    keys.forEach(key => {
        allowCredentials.push({
            type: "public-key",
            id: key
        })
    });

    const result = await get({
        publicKey: {
            challenge: base64Challenge,
            rpId: relyingPartyId,
            allowCredentials: allowCredentials
        }
    })

    let encodedResult = {
        id: result.id,
        rawId: result.rawId,
        type: result.type,
        response: {
            authenticatorData: result.response.authenticatorData,
            signature: result.response.signature,
            userHandle: result.response.userHandle,
            clientDataJSON: result.response.clientDataJSON
        }
    }

    ajax({
        url: completeLoginCallback,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(encodedResult),
        success: function () {
            window.location.href = "/";
        }
    });
}


export async function register(base64Challenge: string, relyingPartyId: string, base64UserHandle: string, userId: string, completeRegistrationCallback: string) {

    // Supported algorithms, ordered by preference
    const pubKeyCredParams: PublicKeyCredentialParameters[] = [
        {
            type: "public-key",
            alg: -8
        },
        {
            type: "public-key",
            alg: -259
        },
        {
            type: "public-key",
            alg: -39
        },
        {
            type: "public-key",
            alg: -36
        },
        {
            type: "public-key",
            alg: -258
        },
        {
            type: "public-key",
            alg: -38
        },
        {
            type: "public-key",
            alg: -35
        },
        {
            type: "public-key",
            alg: -7
        },
        {
            type: "public-key",
            alg: -257
        },
        {
            type: "public-key",
            alg: -37
        },
        {
            type: "public-key",
            alg: -7
        },
        {
            type: "public-key",
            alg: -65535
        }
    ];

    // Relying party details
    let rp: PublicKeyCredentialRpEntity = {
        id: relyingPartyId,
        name: "FIDO Test Video"
    };

    let user = {
        name: userId,
        displayName: userId,
        id: base64UserHandle
    };

    const credentials = await create({
        publicKey: {
            challenge: base64Challenge,
            rp: rp,
            user: user,
            pubKeyCredParams: pubKeyCredParams
        }
    });

    let encodedCredentials = {
        id: credentials.id,
        rawId: credentials.rawId,
        type: credentials.type,
        response: {
            attestationObject: credentials.response.attestationObject,
            clientDataJSON: credentials.response.clientDataJSON
        }
    };

    // post to register callback endpoint and redirect to homepage
    ajax({
        url: completeRegistrationCallback,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(encodedCredentials),
        success: function () {
            window.location.href = "/";
        },
        error: function (e) {
            console.error("Error from server... " + e);
        }
    });

}
