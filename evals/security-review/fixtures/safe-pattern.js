function loadProfile(userId) {
  const query = "SELECT display_name FROM users WHERE id = ?";
  return database.execute(query, [userId]);
}

function showDisplayName(displayName, output) {
  output.textContent = displayName;
}

module.exports = { loadProfile, showDisplayName };
