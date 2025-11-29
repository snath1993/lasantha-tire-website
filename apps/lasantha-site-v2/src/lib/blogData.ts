export const blogPosts = [
  {
    title: 'When Should You Replace Your Tyres?',
    titleSi: 'ඔබේ ටයර් මාරු කළ යුතු නිවැරදි කාලය හඳුනා ගනිමු',
    excerpt: 'Learn the warning signs that indicate it\'s time for new tyres. From tread depth to age, we cover everything you need to know.',
    excerptSi: 'නව ටයර් යෙදීමට කාලය එළඹ ඇති බව පෙන්වන අනතුරු ඇඟවීම් හඳුනා ගන්න. ට්‍රෙඩ් (Tread) ගැඹුරේ සිට ටයරයේ කල් පැවැත්ම දක්වා ඔබ දැනගත යුතු සියල්ල මෙහි අඩංගු වේ.',
    content: `
      <h2>Warning Signs You Need New Tyres</h2>
      <p>Driving with worn-out tyres is dangerous. Here are the key signs to look for:</p>
      <ul>
        <li><strong>Tread Depth:</strong> If your tread depth is below 1.6mm, it's illegal and unsafe. Use a coin to check.</li>
        <li><strong>Cracks in the Sidewall:</strong> Look for cuts or tracks in the sidewall. This could signal a developing leak or a potential blowout.</li>
        <li><strong>Bulges and Blisters:</strong> If you see a bulge extending outward from the rest of the surface, it means the outer surface has weakened.</li>
        <li><strong>Vibration:</strong> Excessive vibration while driving could indicate tyre damage or alignment issues.</li>
      </ul>
      <p>If you notice any of these signs, visit Lasantha Tyre Traders immediately for a professional inspection.</p>
    `,
    contentSi: `
      <h2>ඔබට නව ටයර් අවශ්‍ය බව පෙන්වන අනතුරු සංඥා</h2>
      <p>ගෙවී ගිය ටයර් සහිතව රිය පැදවීම ඉතා අනතුරුදායකය. ඔබ සැලකිලිමත් විය යුතු ප්‍රධාන කරුණු පහත දැක්වේ:</p>
      <ul>
        <li><strong>ට්‍රෙඩ් (Tread) ගැඹුර:</strong> ටයරයේ කට්ට වල ගැඹුර 1.6mm ට වඩා අඩු නම්, එය නීති විරෝධී මෙන්ම අනාරක්ෂිත වේ. මෙය පරීක්ෂා කිරීමට කාසියක් භාවිතා කළ හැක.</li>
        <li><strong>පැති බැම්මේ (Sidewall) ඉරිතැලීම්:</strong> ටයරයේ පැති බැම්මේ කැපුම් හෝ ඉරිතැලීම් තිබේදැයි පරීක්ෂා කරන්න. මෙය වාතය කාන්දු වීමක් හෝ ටයරය පිපිරීමක් (Blowout) ඇති වීමට හේතු විය හැක.</li>
        <li><strong>ඉදිමීම් සහ බිබිලි:</strong> ටයරයේ මතුපිටින් පිටතට නෙරා ගිය ඉදිමීමක් දක්නට ලැබේ නම්, එයින් අදහස් වන්නේ ටයරයේ අභ්‍යන්තර ව්‍යුහය දුර්වල වී ඇති බවයි.</li>
        <li><strong>අධික කම්පනය:</strong> රිය පැදවීමේදී අසාමාන්‍ය කම්පනයක් දැනේ නම්, එය ටයර් වල හානියක් හෝ රෝද වල Alignment ගැටළුවක් විය හැක.</li>
      </ul>
      <p>ඔබ මෙම ලක්ෂණ කිසිවක් නිරීක්ෂණය කළහොත්, වහාම ලසන්ත ටයර් ට්‍රේඩර්ස් වෙත පැමිණ වෘත්තීය මට්ටමේ පරීක්ෂාවක් සිදු කරවා ගන්න.</p>
    `,
    image: '/images/blog/tyre-replacement.svg',
    category: 'Maintenance',
    readTime: '5 min read',
    slug: 'when-to-replace-tyres'
  },
  {
    title: 'Understanding Tyre Markings: A Complete Guide',
    titleSi: 'ටයර් වල ඇති සංකේත තේරුම් ගනිමු: සම්පූර්ණ මාර්ගෝපදේශයක්',
    excerpt: 'What does 195/65R15 91V mean? Decode the numbers and letters on your tyre sidewall.',
    excerptSi: '195/65R15 91V යන්නෙන් අදහස් කරන්නේ කුමක්ද? ඔබේ ටයරයේ පැති බැම්මේ ඇති අංක සහ අකුරු වල තේරුම දැනගන්න.',
    content: `
      <h2>Decoding Your Tyre Sidewall</h2>
      <p>The numbers on your tyre (e.g., 195/65R15 91V) tell you everything about its size and capabilities:</p>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 my-8 not-prose">
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">195</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Tyre Width</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Width of the tyre in millimeters.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">65</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Aspect Ratio</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Height is 65% of the width.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">R</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Construction</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Radial construction.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">15</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Rim Diameter</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Wheel diameter in inches.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">91</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Load Index</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Maximum weight capacity.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-red-600 mb-2">V</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Speed Rating</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed">Maximum speed capability.</p>
        </div>
      </div>

      <h3>Common Speed Ratings</h3>
      <p>The last letter indicates the maximum speed the tyre can sustain:</p>
      
      <div class="grid grid-cols-2 md:grid-cols-3 gap-3 my-6 not-prose">
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">S</span>
          <span class="text-sm font-medium text-gray-800">180 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Family Sedans</span>
        </div>
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">T</span>
          <span class="text-sm font-medium text-gray-800">190 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Sedans/Vans</span>
        </div>
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">H</span>
          <span class="text-sm font-medium text-gray-800">210 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Sport Sedans</span>
        </div>
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">V</span>
          <span class="text-sm font-medium text-gray-800">240 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Sports Cars</span>
        </div>
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">W</span>
          <span class="text-sm font-medium text-gray-800">270 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Exotic Cars</span>
        </div>
        <div class="bg-gray-50 p-3 rounded-lg text-center border border-gray-200">
          <span class="block text-2xl font-bold text-red-600">Y</span>
          <span class="text-sm font-medium text-gray-800">300 km/h</span>
          <span class="block text-xs text-gray-500 mt-1">Exotic Cars</span>
        </div>
      </div>

      <h3>DOT Code: How Old is Your Tyre?</h3>
      <p>Look for a 4-digit code stamped on the sidewall (e.g., 2423).</p>
      
      <div class="bg-blue-50 p-6 rounded-xl border border-blue-100 my-6 not-prose">
        <div class="flex flex-col items-center justify-center mb-6">
          <div class="bg-white px-8 py-4 rounded-lg border-2 border-blue-200 text-4xl font-mono font-bold tracking-[0.2em] text-gray-800 shadow-sm flex gap-1">
            <span class="text-blue-600">24</span><span class="text-green-600">23</span>
          </div>
          <span class="text-xs text-gray-500 mt-2 uppercase tracking-wide">Example Code</span>
        </div>
        <div class="grid grid-cols-2 gap-4 text-sm">
          <div class="text-center p-3 bg-white rounded-lg border border-blue-100">
            <span class="block font-bold text-blue-600 text-xl mb-1">24</span>
            <span class="text-gray-800 font-medium">Week of Manufacture</span>
            <span class="block text-xs text-gray-500 mt-1">(24th Week)</span>
          </div>
          <div class="text-center p-3 bg-white rounded-lg border border-green-100">
            <span class="block font-bold text-green-600 text-xl mb-1">23</span>
            <span class="text-gray-800 font-medium">Year of Manufacture</span>
            <span class="block text-xs text-gray-500 mt-1">(2023)</span>
          </div>
        </div>
        <div class="mt-4 text-center">
            <p class="text-sm text-blue-800 font-medium m-0 bg-blue-100 py-2 px-4 rounded-full inline-block">⚠️ Recommendation: Replace tyres older than 5-6 years.</p>
        </div>
      </div>

      <h3>UTQG Ratings</h3>
      <p>Uniform Tire Quality Grading (UTQG) provides info on:</p>
      
      <div class="space-y-3 my-6 not-prose">
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-orange-100 text-orange-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">1</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Treadwear</h5>
            <p class="text-sm text-gray-600 m-0 mt-1">Higher number means longer lasting (e.g., 400 lasts longer than 200).</p>
          </div>
        </div>
        
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-blue-100 text-blue-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">2</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Traction</h5>
            <p class="text-sm text-gray-600 m-0 mt-1">Grades AA, A, B, C. <span class="font-semibold text-blue-600">AA</span> offers the best grip on wet roads.</p>
          </div>
        </div>
        
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-red-100 text-red-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">3</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Temperature</h5>
            <p class="text-sm text-gray-600 m-0 mt-1">Grades A, B, C. <span class="font-semibold text-red-600">A</span> resists heat buildup best.</p>
          </div>
        </div>
      </div>

      <p>Understanding these helps you choose the right replacement tyres for your vehicle.</p>
    `,
    contentSi: `
      <h2>ටයර් පැති බැම්මේ (Sidewall) ඇති තොරතුරු කියවීම</h2>
      <p>ඔබේ ටයරයේ සඳහන් අංක (උදා: 195/65R15 91V) මගින් එහි ප්‍රමාණය සහ කාර්ය සාධනය පිළිබඳ සියලු තොරතුරු හෙළි කරයි:</p>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 my-8 not-prose">
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">195</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">ටයරයේ පළල</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">ටයරයේ පළල මිලිමීටර වලින්.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">65</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">Aspect Ratio</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">ටයරයේ උස, එහි පළලෙන් 65% කි.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">R</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">තාක්ෂණය</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">රේඩියල් (Radial) තාක්ෂණය.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">15</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">විෂ්කම්භය</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">රෝදයේ විෂ්කම්භය අඟල් වලින්.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">91</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">Load Index</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">ටයරයට දැරිය හැකි උපරිම බර.</p>
        </div>
        
        <div class="bg-white p-5 rounded-2xl border border-amber-100 shadow-sm hover:shadow-md transition-all hover:-translate-y-1">
          <div class="text-4xl font-extrabold text-amber-600 mb-2">V</div>
          <div class="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2 font-sinhala">Speed Rating</div>
          <p class="text-gray-700 text-sm m-0 leading-relaxed font-sinhala">ටයරයට ඔරොත්තු දෙන උපරිම වේගය.</p>
        </div>
      </div>

      <h3>වේග ශ්‍රේණිගත කිරීම් (Speed Ratings)</h3>
      <p>ටයරයේ සඳහන් අවසාන ඉංග්‍රීසි අකුරෙන් (උදා: V) දැක්වෙන්නේ එයට ඔරොත්තු දෙන උපරිම වේගයයි:</p>
      
      <div class="grid grid-cols-2 md:grid-cols-3 gap-3 my-6 not-prose">
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">S</span>
          <span class="text-sm font-medium text-gray-800">180 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">සාමාන්‍ය කාර්</span>
        </div>
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">T</span>
          <span class="text-sm font-medium text-gray-800">190 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">කාර්/වෑන්</span>
        </div>
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">H</span>
          <span class="text-sm font-medium text-gray-800">210 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">Sport Sedans</span>
        </div>
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">V</span>
          <span class="text-sm font-medium text-gray-800">240 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">Sports Cars</span>
        </div>
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">W</span>
          <span class="text-sm font-medium text-gray-800">270 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">Exotic Cars</span>
        </div>
        <div class="bg-amber-50 p-3 rounded-lg text-center border border-amber-200">
          <span class="block text-2xl font-bold text-amber-700">Y</span>
          <span class="text-sm font-medium text-gray-800">300 km/h</span>
          <span class="block text-xs text-gray-600 mt-1 font-sinhala">Super Cars</span>
        </div>
      </div>

      <h3>DOT කේතය: ඔබේ ටයරය කොපමණ පැරණිද?</h3>
      <p>ටයරයේ පැති බැම්මේ ඇති ඉලක්කම් 4ක කේතය සොයන්න (උදා: <strong>2423</strong>).</p>
      
      <div class="bg-blue-50 p-6 rounded-xl border border-blue-100 my-6 not-prose">
        <div class="flex flex-col items-center justify-center mb-6">
          <div class="bg-white px-8 py-4 rounded-lg border-2 border-blue-200 text-4xl font-mono font-bold tracking-[0.2em] text-gray-800 shadow-sm flex gap-1">
            <span class="text-blue-600">24</span><span class="text-green-600">23</span>
          </div>
          <span class="text-xs text-gray-500 mt-2 uppercase tracking-wide font-sinhala">උදාහරණ කේතය</span>
        </div>
        <div class="grid grid-cols-2 gap-4 text-sm">
          <div class="text-center p-3 bg-white rounded-lg border border-blue-100">
            <span class="block font-bold text-blue-600 text-xl mb-1">24</span>
            <span class="text-gray-800 font-medium font-sinhala">නිෂ්පාදිත සතිය</span>
            <span class="block text-xs text-gray-500 mt-1 font-sinhala">(24 වන සතිය)</span>
          </div>
          <div class="text-center p-3 bg-white rounded-lg border border-green-100">
            <span class="block font-bold text-green-600 text-xl mb-1">23</span>
            <span class="text-gray-800 font-medium font-sinhala">නිෂ්පාදිත වර්ෂය</span>
            <span class="block text-xs text-gray-500 mt-1 font-sinhala">(2023)</span>
          </div>
        </div>
        <div class="mt-4 text-center">
            <p class="text-sm text-blue-800 font-medium m-0 bg-blue-100 py-2 px-4 rounded-full inline-block font-sinhala">⚠️ නිර්දේශය: වසර 5-6 කට වඩා පැරණි ටයර් මාරු කරන්න.</p>
        </div>
      </div>

      <h3>UTQG ශ්‍රේණිගත කිරීම්</h3>
      <p>මෙමගින් ටයරයේ ගුණාත්මකභාවය පිළිබඳ අමතර තොරතුරු ලබා දෙයි:</p>
      
      <div class="space-y-3 my-6 not-prose">
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-orange-100 text-orange-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">1</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Treadwear</h5>
            <p class="text-sm text-gray-600 m-0 mt-1 font-sinhala">අංකය වැඩි වන තරමට ටයරය ගෙවී යාම අඩුය (උදා: 200 ට වඩා 400 කල් පවතී).</p>
          </div>
        </div>
        
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-blue-100 text-blue-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">2</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Traction</h5>
            <p class="text-sm text-gray-600 m-0 mt-1 font-sinhala">AA, A, B, C ලෙස වර්ග කෙරේ. <span class="font-semibold text-blue-600">AA</span> යනු තෙත් මාර්ගවල හොඳම ග්‍රහණයයි.</p>
          </div>
        </div>
        
        <div class="flex items-start gap-4 p-4 bg-white rounded-xl border border-gray-100 shadow-sm">
          <div class="bg-red-100 text-red-600 w-12 h-12 flex items-center justify-center rounded-lg font-bold text-xl shrink-0">3</div>
          <div>
            <h5 class="font-bold text-gray-900 m-0 text-lg">Temperature</h5>
            <p class="text-sm text-gray-600 m-0 mt-1 font-sinhala">A, B, C ලෙස වර්ග කෙරේ. <span class="font-semibold text-red-600">A</span> යනු තාපයට ඔරොත්තු දීමේ ඉහළම හැකියාවයි.</p>
          </div>
        </div>
      </div>

      <p>මෙම කරුණු අවබෝධ කර ගැනීම, ඔබේ වාහනයට වඩාත් සුදුසු ටයර් තෝරා ගැනීමට ඔබට උපකාරී වනු ඇත.</p>
    `,
    image: '/images/blog/tyre-markings.svg',
    category: 'Education',
    readTime: '7 min read',
    slug: 'tyre-markings-guide'
  },
  {
    title: 'Fuel Saving Tyres: Do They Really Work?',
    titleSi: 'ඉන්ධන ඉතිරි කරන ටයර්: ඒවා සැබවින්ම ඵලදායීද?',
    excerpt: 'Discover how low rolling resistance tyres can save you money on fuel costs in Sri Lankan driving conditions.',
    excerptSi: 'ශ්‍රී ලංකාවේ මාර්ග තත්වයන් යටතේ, අඩු Rolling Resistance සහිත ටයර් භාවිතයෙන් ඉන්ධන වියදම අඩු කරගන්නේ කෙසේදැයි සොයා බලමු.',
    content: `
      <h2>How Low Rolling Resistance Tyres Save Fuel</h2>
      <p>Fuel-saving tyres are designed to reduce the energy lost as the tyre rolls. This is called "rolling resistance".</p>
      <p>By using special rubber compounds and tread patterns, these tyres require less energy to move, which means your engine doesn't have to work as hard.</p>
      <h3>Benefits:</h3>
      <ul>
        <li>Improved fuel economy (up to 4-6% savings).</li>
        <li>Reduced CO2 emissions.</li>
        <li>Often quieter and smoother ride.</li>
      </ul>
      <p>In Sri Lanka's stop-and-go traffic, these savings can add up significantly over the life of the tyre.</p>
    `,
    contentSi: `
      <h2>අඩු Rolling Resistance සහිත ටයර් මගින් ඉන්ධන ඉතිරි වන්නේ කෙසේද?</h2>
      <p>ඉන්ධන ඉතිරි කරන ටයර් නිර්මාණය කර ඇත්තේ ටයරය ධාවනය වන විට සිදුවන ශක්ති හානිය අවම කිරීමටයි. මෙය "Rolling Resistance" ලෙස හැඳින්වේ.</p>
      <p>විශේෂ රබර් සංයෝග සහ ට්‍රෙඩ් (Tread) රටා භාවිතා කිරීම මගින්, මෙම ටයර් ධාවනය වීමට වැය වන ශක්තිය අඩු කරයි. එමගින් එන්ජිම මත යෙදෙන අමතර පීඩනය අඩු වේ.</p>
      <h3>වාසි:</h3>
      <ul>
        <li>ඉන්ධන කාර්යක්ෂමතාව ඉහළ යාම (4-6% දක්වා ඉතිරියක්).</li>
        <li>කාබන් ඩයොක්සයිඩ් (CO2) විමෝචනය අඩු වීම.</li>
        <li>වඩාත් නිහඬ සහ සුමට ධාවනයක් ලබා දීම.</li>
      </ul>
      <p>ශ්‍රී ලංකාවේ වාහන තදබදය සහිත මාර්ගවල ධාවනය කිරීමේදී, ටයරයේ ආයු කාලය තුළ මෙම ඉතිරිකිරීම සැලකිය යුතු අගයක් ගනු ඇත.</p>
    `,
    image: '/images/blog/fuel-saving.svg',
    category: 'Tips',
    readTime: '6 min read',
    slug: 'fuel-saving-tyres'
  },
  {
    title: 'Wheel Alignment vs Balancing: What\'s the Difference?',
    titleSi: 'Wheel Alignment සහ Wheel Balancing අතර වෙනස කුමක්ද?',
    excerpt: 'Many people confuse these two services. Learn why both are crucial for your vehicle\'s performance and tyre life.',
    excerptSi: 'බොහෝ දෙනෙක් මෙම සේවාවන් දෙක පටලවා ගනිති. ඔබේ වාහනයේ ක්‍රියාකාරීත්වයට සහ ටයර් වල කල් පැවැත්මට මෙම සේවාවන් දෙකම අත්‍යවශ්‍ය වන්නේ ඇයිද යන්න දැනගන්න.',
    content: `
      <h2>Alignment vs. Balancing</h2>
      <p>While often performed together, these are two distinct services:</p>
      <h3>Wheel Alignment</h3>
      <p>Adjusting the angles of the tyres so they are parallel to each other and perpendicular to the ground. This prevents uneven wear and keeps the vehicle driving straight.</p>
      <h3>Wheel Balancing</h3>
      <p>Ensuring that weight is distributed evenly around the wheel. Unbalanced wheels cause vibration, usually felt in the steering wheel at higher speeds.</p>
      <p>Both are essential for a smooth ride and long tyre life.</p>
    `,
    contentSi: `
      <h2>Alignment සහ Balancing</h2>
      <p>බොහෝ විට මෙම සේවාවන් දෙක එකවර සිදු කළද, මේවා එකිනෙකට වෙනස් ක්‍රියාවලීන් දෙකකි:</p>
      <h3>Wheel Alignment</h3>
      <p>රෝද එකිනෙකට සමාන්තරව සහ පොළවට ලම්බකව පවතින පරිදි ඒවායේ කෝණ (Angles) නිවැරදි කිරීම මෙහිදී සිදුවේ. මෙය ටයර් අසමාන ලෙස ගෙවී යාම වළක්වන අතර වාහනය ඍජුව ධාවනය කිරීමට උපකාරී වේ.</p>
      <h3>Wheel Balancing</h3>
      <p>රෝදය වටා බර සමානව බෙදා හැරීම සහතික කිරීම මෙහිදී සිදුවේ. සමබර නොවන රෝද මගින් කම්පනයක් ඇති කරන අතර, එය සාමාන්‍යයෙන් අධික වේගයේදී සුක්කානම (Steering Wheel) හරහා දැනිය හැක.</p>
      <p>සුමට ධාවනයක් සහ දිගු ටයර් ආයු කාලයක් සඳහා මෙම සේවාවන් දෙකම අත්‍යවශ්‍ය වේ.</p>
    `,
    image: '/images/blog/alignment-balancing.svg',
    category: 'Services',
    readTime: '4 min read',
    slug: 'alignment-vs-balancing'
  },
  {
    title: 'Best Tyres for Rainy Season in Sri Lanka',
    titleSi: 'ශ්‍රී ලංකාවේ වැසි කාලයට වඩාත් සුදුසු ටයර්',
    excerpt: 'Monsoon season demands special attention. Find out which tyre brands and patterns perform best on wet roads.',
    excerptSi: 'මෝසම් වැසි සමයේදී රිය පැදවීම ගැන විශේෂ අවධානයක් යොමු කළ යුතුය. තෙත් මාර්ගවල ඉහළම ක්‍රියාකාරීත්වයක් ලබා දෙන ටයර් වර්ග සහ රටා (Patterns) ගැන දැනගන්න.',
    content: `
      <h2>Driving Safely in the Monsoon</h2>
      <p>Wet roads reduce grip significantly. You need tyres designed to channel water away quickly to prevent hydroplaning.</p>
      <h3>What to Look For:</h3>
      <ul>
        <li><strong>Deep Grooves:</strong> Help evacuate water efficiently.</li>
        <li><strong>Silica Compound:</strong> Improves grip on wet surfaces.</li>
        <li><strong>Directional Tread Patterns:</strong> Often better at cutting through water.</li>
      </ul>
      <p>Brands like Michelin, Bridgestone, and GT Radial offer excellent wet-weather options suitable for Sri Lankan roads.</p>
    `,
    contentSi: `
      <h2>මෝසම් වැසි සමයේ ආරක්ෂිතව රිය පැදවීම</h2>
      <p>තෙත් මාර්ග වලදී ටයර් වල ග්‍රහණය (Grip) සැලකිය යුතු ලෙස අඩු වේ. ජලය ඉක්මනින් ඉවත් කිරීමට සහ ලිස්සා යාම (Hydroplaning) වැළැක්වීමට විශේෂයෙන් නිර්මාණය කර ඇති ටයර් භාවිතය ඉතා වැදගත් වේ.</p>
      <h3>සැලකිලිමත් විය යුතු කරුණු:</h3>
      <ul>
        <li><strong>ගැඹුරු කානු (Deep Grooves):</strong> ජලය කාර්යක්ෂමව ඉවත් කිරීමට උපකාරී වේ.</li>
        <li><strong>සිලිකා සංයෝගය (Silica Compound):</strong> තෙත් මතුපිටක් මත ග්‍රහණය වැඩි දියුණු කරයි.</li>
        <li><strong>දිශානුගත ට්‍රෙඩ් රටා (Directional Tread Patterns):</strong> ජලය කපා හැරීමට සහ ඉවත් කිරීමට වඩාත් යෝග්‍ය වේ.</li>
      </ul>
      <p>Michelin, Bridgestone සහ GT Radial වැනි සන්නාම, ශ්‍රී ලංකාවේ මාර්ග සහ කාලගුණ තත්වයන්ට ගැලපෙන විශිෂ්ට ටයර් මාදිලි ලබා දෙයි.</p>
    `,
    image: '/images/blog/rainy-season.svg',
    category: 'Seasonal',
    readTime: '5 min read',
    slug: 'best-tyres-rainy-season'
  },
  {
    title: 'Tyre Pressure: Why It Matters More Than You Think',
    titleSi: 'ටයර් පීඩනය (Tyre Pressure): ඔබ සිතනවාට වඩා එය වැදගත් වන්නේ ඇයි?',
    excerpt: 'Proper tyre pressure affects safety, fuel economy, and tyre lifespan. Learn how to check and maintain it.',
    excerptSi: 'නිවැරදි ටයර් පීඩනය ඔබේ ආරක්ෂාව, ඉන්ධන කාර්යක්ෂමතාව සහ ටයර් වල ආයු කාලය කෙරෙහි සෘජුවම බලපායි. එය පරීක්ෂා කර නඩත්තු කරන ආකාරය ඉගෙන ගන්න.',
    content: `
      <h2>The Importance of Correct Tyre Pressure</h2>
      <p>Maintaining the right tyre pressure is the single most important thing you can do for your tyres.</p>
      <h3>Why it matters:</h3>
      <ul>
        <li><strong>Safety:</strong> Under-inflated tyres overheat and can blow out. Over-inflated tyres have less grip.</li>
        <li><strong>Economy:</strong> Correct pressure reduces rolling resistance, saving fuel.</li>
        <li><strong>Longevity:</strong> Improper pressure causes uneven wear, shortening tyre life.</li>
      </ul>
      <p>Check your pressure at least once a month when tyres are cold.</p>
    `,
    contentSi: `
      <h2>නිවැරදි ටයර් පීඩනය පවත්වා ගැනීමේ වැදගත්කම</h2>
      <p>නිවැරදි ටයර් පීඩනය පවත්වා ගැනීම, ඔබේ ටයර් වල කල් පැවැත්ම සඳහා ඔබට කළ හැකි වැදගත්ම කාර්යයයි.</p>
      <h3>එය වැදගත් වීමට හේතු:</h3>
      <ul>
        <li><strong>ආරක්ෂාව:</strong> නියමිත ප්‍රමාණයට වඩා අඩු පීඩනයක් සහිත ටයර් අධික ලෙස රත් වී පුපුරා යා හැක. එමෙන්ම වැඩි පීඩනයක් සහිත ටයර්වල මාර්ගය සමඟ ඇති ග්‍රහණය (Grip) අඩු වේ.</li>
        <li><strong>ඉන්ධන පිරිමැස්ම:</strong> නිවැරදි පීඩනය මගින් Rolling Resistance අඩු කරන අතර, එමගින් ඉන්ධන ඉතිරි වේ.</li>
        <li><strong>කල් පැවැත්ම:</strong> වැරදි පීඩනය නිසා ටයර් අසමාන ලෙස ගෙවී යන අතර, එමගින් ටයර් වල ආයු කාලය කෙටි වේ.</li>
      </ul>
      <p>ටයර් සිසිල්ව පවතින විට (ධාවනයට පෙර), අවම වශයෙන් මසකට වරක්වත් ටයර් පීඩනය පරීක්ෂා කරන්න.</p>
    `,
    image: '/images/blog/tyre-pressure.svg',
    category: 'Maintenance',
    readTime: '4 min read',
    slug: 'tyre-pressure-guide'
  }
]
