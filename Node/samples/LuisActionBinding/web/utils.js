var crypto = require('crypto');
var algo = 'aes-256-ctr';
var password = 'itsasecret!';

function encrypt(input) {
    var cipher = crypto.createCipher(algo, password);
    var encrypted = cipher.update(input, 'utf8', 'hex');
    encrypted += cipher.final('hex');
    return encrypted;
}

function descrypt(cryptedInput) {
    var decipher = crypto.createDecipher(algo, password);
    var decryted = decipher.update(cryptedInput, 'hex', 'utf8');
    decryted += decipher.final('utf8');
    return decryted;
}

module.exports = {
    serialize: function (o) {
        return encrypt(JSON.stringify(o));
    },
    deserialize: function (s) {
        return JSON.parse(descrypt(s));
    }
};