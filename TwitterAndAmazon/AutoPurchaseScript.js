/*
  REQUIRED TO MAKE THIS SCRIPT WORK:
    1. You will need Node.JS for this project. It is super common, so if you are working in 
        JavaScript frequently you may already have it, but if not, go download it here.
        https://nodejs.org/en/

    2. Create your project folder and import it into your IDE (VSCode preffered)

    3. You need to initiallize node. Using the powershell or terminal change the directory to your
        project folder using 'cd {FOLDERPATH}'
        e.g. - 'cd C:\Users\{USERNAME}\{FOLDER}\{FOLDER ETC}\{PROJECT FOLDER}'
        If using VSCode hit CTRL+SHIFT+` and it will open a powershell in your directory automatically.
        Once in your folders directory type 'npm init' and it will ask you a few questions.
        You can leave all questions blank and click 'enter' except for 'entry point'. 
        Change the entry point to the name of the script you intend to create. 
        By default it will use '{YOURFOLDERNAME}.js'. I would recommend
        changing this to avoid confusion.

    4. Create a script and name it the same as you put as the 'entry point'.

    4. https://www.npmjs.com/package/selenium-webdriver
        Go to the link above and download the proper driver for your preffered browser.
        Mine is Chrome, so I downloaded 'chromedriver.exe 107.0.5112.79'.
        Extract your driver into your project folder or else where and copy the path.
        http://chromedriver.storage.googleapis.com/index.html?path=104.0.5112.79/

    5. You have to have the folder you extracted the driver to set as an environment variable.
        This is needed so when selenium is looking for the chrome driver, it doesn't need it's
        exact path.
        You can do this by pressing the windows key and searching for 'variables'.
        Click "Environment Variables".
        Either under 'user' or 'system' variables, find 'path'.
        Click 'path' then click 'edit'.
        Click 'new' and add your FULL PATH that you copied earlier. Then click either apply or ok and exit.

    6. With everything installed, now we can download Selenium.
        If in Visual Studio Code hit CTRL+SHIFT+` and it will open a new powershell.
        Inside the powershell type 'npm install selenium-webdriver' and hit enter.
        This will install selenium into your project folder.
        This can also be done through terminal or powershell seperately using the same method
        used above to initiallize node.

  That's it. Have fun! Here's a few helpful links if you need additional help...
    CodingSrc Video Tutorial: https://www.youtube.com/watch?v=QwymPtk4zWo
    https://www.selenium.dev/documentation/
*/

const {Builder, By, Key, util, until} = require("selenium-webdriver");
ASIN = ''; // example: B00NO73Q84
let link = `https://www.amazon.com/gp/aws/cart/add.html?AssociateTag=Associate+Tag&ASIN.1=${ASIN}&Quantity.1=1&add=add`;
let driver;
async function example() {
  driver = await new Builder().forBrowser("chrome").build();
  await driver.get(link);
  await driver.findElement(By.className("a-button-input")).sendKeys(Key.RETURN);
  await driver.findElement(By.name("proceedToRetailCheckout")).sendKeys(Key.RETURN);
  await driver.findElement(By.name("email")).sendKeys("EMAIL", Key.RETURN); // login email
  await driver.findElement(By.name("password")).sendKeys("PASSWORD", Key.RETURN); //login password
  await driver.wait(until.elementLocated(By.xpath('//input[@aria-labelledby="orderSummaryPrimaryActionBtn-announce"]')),10000).click();
  let TotalElement = await driver.wait(until.elementLocated(By.className("a-color-price a-size-medium a-text-right grand-total-price aok-nowrap a-text-bold a-nowrap")),10000);
  let TotalText = (await TotalElement.getText()).substring(1);
  if (parseFloat(TotalText) <= 20) {
    // Timeout necessary to allow page to full load
    //Place Order - Uncomment line below for functionality.
    // setTimeout(PlaceOrder, 5000);
  }
}
function PlaceOrder() {
  driver.findElement(By.name("placeYourOrder1")).sendKeys(Key.RETURN);
}
example();
