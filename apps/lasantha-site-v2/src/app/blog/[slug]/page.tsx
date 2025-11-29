import React from 'react'
import Image from 'next/image'
import Link from 'next/link'
import { notFound } from 'next/navigation'
import { ArrowLeft, Calendar, Clock, Tag, Share2, Facebook, Twitter, Linkedin, Info, TrendingUp } from 'lucide-react'
import { blogPosts } from '@/lib/blogData'
import Header from '@/components/Header'
import Footer from '@/components/Footer'
import { Metadata } from 'next'

interface BlogPostPageProps {
  params: Promise<{
    slug: string
  }>
}

export async function generateStaticParams() {
  return blogPosts.map((post) => ({
    slug: post.slug,
  }))
}

export async function generateMetadata({ params }: BlogPostPageProps): Promise<Metadata> {
  const { slug } = await params
  const post = blogPosts.find((p) => p.slug === slug)

  if (!post) {
    return {
      title: 'Post Not Found',
    }
  }

  return {
    title: post.title,
    description: post.excerpt,
    openGraph: {
      title: post.title,
      description: post.excerpt,
      type: 'article',
      publishedTime: new Date().toISOString(),
      authors: ['Lasantha Tyre Traders'],
      images: [
        {
          url: post.image,
          width: 1200,
          height: 630,
          alt: post.title,
        },
      ],
    },
    alternates: {
      canonical: `/blog/${post.slug}`,
    }
  }
}

export default async function BlogPostPage({ params }: BlogPostPageProps) {
  const { slug } = await params
  const post = blogPosts.find((p) => p.slug === slug)

  if (!post) {
    notFound()
  }

  // Get related posts from the same category
  const relatedPosts = blogPosts
    .filter((p) => p.category === post.category && p.slug !== post.slug)
    .slice(0, 3)

  // Determine if we should show Sinhala content (defaulting to English for now, 
  // but in a real app this might be controlled by a context or URL parameter.
  // Since this is a static page, we'll render both or use a client component wrapper if we want dynamic toggling.
  // For this implementation, I'll render the English content primarily but include Sinhala translations where available
  // or perhaps just render the English version as the base and maybe add a client-side toggle later if requested.
  // Given the user's previous request for a toggle in the card view, let's make this page bilingual-friendly 
  // or just stick to English for the main content structure for now, as the data structure has separate fields.
  
  // Actually, a better approach for the detail page is to show the content based on a client-side state 
  // or just show English for now as the "Read More" link doesn't carry state. 
  // However, to make it nice, I will create a Client Component wrapper for the content area to handle the language toggle.
  
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      
      <main className="pt-32 pb-16 bg-gradient-to-b from-gray-50 to-white">
        <article className="container mx-auto px-4 max-w-5xl">
          {/* Breadcrumb & Back Link */}
          <div className="mb-8 flex items-center justify-between">
            <Link 
              href="/#knowledge-center" 
              className="inline-flex items-center text-gray-600 hover:text-red-600 transition-colors group"
            >
              <ArrowLeft className="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform" />
              Back to Knowledge Center
            </Link>
            <div className="hidden md:flex items-center gap-2 text-sm text-gray-500">
              <span>Home</span>
              <span>/</span>
              <span>Blog</span>
              <span>/</span>
              <span className="text-red-600 font-medium">{post.title.slice(0, 30)}...</span>
            </div>
          </div>

          {/* Hero Image */}
          <div className="relative w-full h-[450px] md:h-[500px] rounded-3xl overflow-hidden mb-8 shadow-2xl group">
            <Image
              src={post.image}
              alt={post.title}
              fill
              className="object-cover group-hover:scale-105 transition-transform duration-700"
              priority
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>
            <div className="absolute top-6 left-6">
              <span className="bg-white/95 backdrop-blur-sm text-red-600 px-5 py-2 rounded-full text-sm font-bold shadow-lg">
                {post.category}
              </span>
            </div>
            <div className="absolute bottom-6 left-6 right-6">
              <div className="flex items-center gap-4 text-white/90 text-sm">
                <div className="flex items-center gap-1.5">
                  <Clock className="w-4 h-4" />
                  <span className="font-medium">{post.readTime}</span>
                </div>
                <div className="w-1 h-1 rounded-full bg-white/60"></div>
                <div className="flex items-center gap-1.5">
                  <Calendar className="w-4 h-4" />
                  <span className="font-medium">{new Date().toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Header Content */}
          <div className="bg-white rounded-3xl p-8 md:p-12 shadow-sm mb-8 border border-gray-100">
            <h1 className="text-3xl md:text-5xl font-extrabold text-gray-900 mb-5 leading-tight">
              {post.title}
            </h1>
            <h2 className="text-xl md:text-3xl font-bold text-gray-600 mb-8 font-sinhala leading-relaxed">
              {post.titleSi}
            </h2>

            <div className="flex flex-wrap items-center gap-6 text-gray-500 text-sm pb-8 mb-8 border-b-2 border-gray-100">
              <div className="flex items-center bg-gray-50 px-4 py-2 rounded-full">
                <Calendar className="w-4 h-4 mr-2 text-red-600" />
                <span className="font-semibold">{new Date().toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })}</span>
              </div>
              <div className="flex items-center bg-gray-50 px-4 py-2 rounded-full">
                <Clock className="w-4 h-4 mr-2 text-red-600" />
                <span className="font-semibold">{post.readTime}</span>
              </div>
              <div className="flex items-center bg-red-50 px-4 py-2 rounded-full">
                <Tag className="w-4 h-4 mr-2 text-red-600" />
                <span className="font-semibold text-red-600">{post.category}</span>
              </div>
            </div>

            {/* Content Body */}
            <div className="prose prose-lg max-w-none text-gray-700">
              {/* Quick Summary Box */}
              <div className="not-prose bg-gradient-to-br from-blue-50 to-indigo-50 border-l-4 border-blue-500 rounded-2xl p-6 mb-8">
                <div className="flex items-start gap-3">
                  <Info className="w-6 h-6 text-blue-600 flex-shrink-0 mt-1" />
                  <div>
                    <h3 className="text-lg font-bold text-gray-900 mb-2">Quick Summary</h3>
                    <p className="text-gray-700 leading-relaxed">{post.excerpt}</p>
                    <p className="text-gray-600 mt-2 font-sinhala text-sm">{post.excerptSi}</p>
                  </div>
                </div>
              </div>

              <div className="[&>h2]:text-2xl [&>h2]:font-bold [&>h2]:text-gray-900 [&>h2]:mt-10 [&>h2]:mb-5 [&>h2]:pb-3 [&>h2]:border-b-2 [&>h2]:border-red-100
                            [&>h3]:text-xl [&>h3]:font-bold [&>h3]:text-gray-800 [&>h3]:mt-8 [&>h3]:mb-4
                            [&>p]:text-base [&>p]:leading-relaxed [&>p]:mb-6 [&>p]:text-gray-700
                            [&>ul]:my-6 [&>ul]:space-y-3
                            [&>ul>li]:flex [&>ul>li]:items-start [&>ul>li]:gap-3
                            [&>ul>li>strong]:text-gray-900 [&>ul>li>strong]:font-bold
                            [&>ul>li::before]:content-['✓'] [&>ul>li::before]:text-green-600 [&>ul>li::before]:font-bold [&>ul>li::before]:text-lg [&>ul>li::before]:flex-shrink-0">
                <div dangerouslySetInnerHTML={{ __html: post.content }} />
              </div>
              
              {/* Sinhala Translation Section */}
              <div className="not-prose mt-12 pt-12 border-t-2 border-gray-200">
                <div className="bg-gradient-to-br from-amber-50 to-orange-50 rounded-3xl p-8 md:p-10 border border-amber-200">
                  <div className="flex items-center gap-3 mb-6">
                    <div className="w-10 h-10 rounded-full bg-amber-500 flex items-center justify-center">
                      <span className="text-white text-xl font-bold">සි</span>
                    </div>
                    <h3 className="text-2xl md:text-3xl font-bold text-gray-900 font-sinhala">සිංහල පරිවර්තනය</h3>
                  </div>
                  <div className="font-sinhala text-base leading-relaxed text-gray-800
                                [&>h2]:text-xl [&>h2]:font-bold [&>h2]:text-gray-900 [&>h2]:mt-8 [&>h2]:mb-4
                                [&>h3]:text-lg [&>h3]:font-bold [&>h3]:text-gray-800 [&>h3]:mt-6 [&>h3]:mb-3
                                [&>p]:mb-5
                                [&>ul]:my-5 [&>ul]:space-y-2.5
                                [&>ul>li]:flex [&>ul>li]:items-start [&>ul>li]:gap-2.5
                                [&>ul>li>strong]:text-gray-900 [&>ul>li>strong]:font-bold
                                [&>ul>li::before]:content-['✓'] [&>ul>li::before]:text-green-600 [&>ul>li::before]:font-bold [&>ul>li::before]:flex-shrink-0" 
                       dangerouslySetInnerHTML={{ __html: post.contentSi }} />
                </div>
              </div>
            </div>
          </div>

          {/* Call to Action */}
          <div className="bg-gradient-to-r from-red-600 to-red-700 rounded-3xl p-8 md:p-10 text-white mb-8">
            <div className="flex flex-col md:flex-row items-center justify-between gap-6">
              <div>
                <h3 className="text-2xl md:text-3xl font-bold mb-2">Need Expert Tyre Advice?</h3>
                <p className="text-red-100 text-lg">Get a free consultation from our tyre specialists today!</p>
              </div>
              <a 
                href="https://wa.me/94721222509?text=Hi,%20I%20read%20your%20article%20and%20need%20advice" 
                target="_blank"
                rel="noopener noreferrer"
                className="bg-white text-red-600 px-8 py-4 rounded-full font-bold text-lg hover:bg-gray-100 transition-all flex items-center gap-2 shadow-xl whitespace-nowrap"
              >
                <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z"/>
                </svg>
                Chat on WhatsApp
              </a>
            </div>
          </div>

          {/* Share Section */}
          <div className="bg-white rounded-3xl p-6 md:p-8 shadow-sm border border-gray-100 mb-8">
            <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
              <span className="font-bold text-gray-900 flex items-center text-lg">
                <Share2 className="w-5 h-5 mr-3 text-red-600" />
                Share this article
              </span>
              <div className="flex gap-3">
                <button className="p-3 rounded-full bg-gradient-to-br from-blue-500 to-blue-600 text-white hover:shadow-lg hover:scale-110 transition-all">
                  <Facebook className="w-5 h-5" />
                </button>
                <button className="p-3 rounded-full bg-gradient-to-br from-sky-400 to-sky-500 text-white hover:shadow-lg hover:scale-110 transition-all">
                  <Twitter className="w-5 h-5" />
                </button>
                <button className="p-3 rounded-full bg-gradient-to-br from-blue-600 to-blue-700 text-white hover:shadow-lg hover:scale-110 transition-all">
                  <Linkedin className="w-5 h-5" />
                </button>
              </div>
            </div>
          </div>

          {/* Related Articles */}
          {relatedPosts.length > 0 && (
            <div className="bg-gray-50 rounded-3xl p-8 md:p-10 border border-gray-200">
              <div className="flex items-center gap-3 mb-8">
                <TrendingUp className="w-6 h-6 text-red-600" />
                <h3 className="text-2xl md:text-3xl font-bold text-gray-900">Related Articles</h3>
              </div>
              <div className="grid md:grid-cols-3 gap-6">
                {relatedPosts.map((relatedPost) => (
                  <Link
                    key={relatedPost.slug}
                    href={`/blog/${relatedPost.slug}`}
                    className="group bg-white rounded-2xl overflow-hidden shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100"
                  >
                    <div className="relative h-40 overflow-hidden">
                      <Image
                        src={relatedPost.image}
                        alt={relatedPost.title}
                        fill
                        className="object-cover group-hover:scale-110 transition-transform duration-500"
                      />
                    </div>
                    <div className="p-5">
                      <div className="flex items-center gap-2 mb-3">
                        <span className="text-xs font-bold text-red-600 bg-red-50 px-3 py-1 rounded-full">
                          {relatedPost.category}
                        </span>
                        <span className="text-xs text-gray-500 flex items-center gap-1">
                          <Clock className="w-3 h-3" />
                          {relatedPost.readTime}
                        </span>
                      </div>
                      <h4 className="font-bold text-gray-900 group-hover:text-red-600 transition-colors line-clamp-2 mb-2">
                        {relatedPost.title}
                      </h4>
                      <p className="text-sm text-gray-600 line-clamp-2">
                        {relatedPost.excerpt}
                      </p>
                    </div>
                  </Link>
                ))}
              </div>
            </div>
          )}
        </article>
      </main>

      <Footer />
    </div>
  )
}
