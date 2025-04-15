
/*
    Non-browser tests.
*/

//todo: It would be ideal to have browser tests in the future.

const { test } = require("uvu");
const assert = require("uvu/assert");
const BuildVariables = require("../ci/src/modules/build_variables");
const HiRpc = require("../ci/src/index");

test("build variables", () => {

    const buildVariables = new BuildVariables.default();

    assert.is(buildVariables.DISABLE_INFO_LOGS, false);
    assert.is(buildVariables.CLIENT_ID, "123456789987654321");
    assert.is(buildVariables.DISABLE_CONSOLE_LOG_OVERRIDE, true);
    assert.is(buildVariables.MAPPINGS.length, 0);
    assert.is(buildVariables.PATCH_URL_MAPPINGS_CONFIG.patchSrcAttributes, false);
    assert.is(buildVariables.OAUTH_SCOPES[0], "identify");
    assert.is(buildVariables.TOKEN_REQUEST_PATH, "/api/token");
    assert.is(buildVariables.SERVER_REQUEST, "");
});

test("fail load", async () => {

    const hiRpc = new HiRpc.default();

    let loaded = false;

    try {
        await hiRpc.load();

        loaded = true;
    }
    catch (_) {}

    assert.is(loaded, false);
});

test.run();