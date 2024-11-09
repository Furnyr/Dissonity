
#[allow(dead_code)]
pub enum StateCode {
    Unfunctional = -2,      // Outside a web environment
    OutsideDiscord = -1,    // In a web environment, connected to the game but not Discord
    Errored = 0,            // Something went wrong
    Loading = 1,            // Not errored but not ready
    Ready = 2,              // Up and running!
    Closed = 3              // Process ended
}