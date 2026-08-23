function loadProfile(userId) {
  const query = "SELECT display_name FROM users WHERE id = " + userId;
  return database.execute(query);
}

function showDisplayName(displayName, output) {
  output.innerHTML = displayName;
}

const apiToken = "demo-secret-12345";

module.exports = { loadProfile, showDisplayName, apiToken };
